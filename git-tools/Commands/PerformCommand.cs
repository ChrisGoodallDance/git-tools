namespace git_tools.Commands;

public abstract class PerformCommand : PerformCommand<GlobalSettings>
{
    protected PerformCommand(Func<GlobalSettings, string> command, IGitService gitService)
        : base(command, gitService)
    {
    }
}

public abstract class PerformCommand<T> : AsyncCommand<T> where T : GlobalSettings
{
    private readonly Func<T, string> _command;
    private readonly IGitService _gitService;

    protected PerformCommand(Func<T, string> command, IGitService gitService)
    {
        _command = command;
        _gitService = gitService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, T settings)
    {
        var command = _command(settings);

        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn("Status");

        await AnsiConsole.Status().StartAsync("Performing " + command + "...", async ctx =>
        {
            var tasks = _gitService
                .GetGitDirectories(settings.Path)
                .Select(d => _gitService.RunAsync(command, settings, d))
                .ToList();

            foreach (var task in tasks)
            {
                var result = await task;
                table.AddRow(result.Directory.Name, GetResultMessage(result));
            }

            AnsiConsole.Write(table);
        });

        return 0;
    }

    protected virtual string GetResultMessage(RunResult result)
    {
        return !string.IsNullOrWhiteSpace(result.Message) ? result.Message.TrimEnd('\n') : result.Status.ToString();
    }
}