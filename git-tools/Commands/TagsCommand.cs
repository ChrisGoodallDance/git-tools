namespace git_tools.Commands;

[Description("Show latest tags on current branch")]
internal sealed class TagsCommand : AsyncCommand<TagsCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<TagsCommand>("tags")
            .WithExample(new [] {"tags", "release" })
            .WithExample(new [] { "tags", "release", "--path", "./path/to/git/repo" });
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [Description("Perform a FETCH operation before searching the tags")]
        [CommandOption("--fetch")]
        [DefaultValue(false)]
        public bool Fetch { get; init; }

        [Description("Show tag with specified prefix")]
        [CommandArgument(0, "[prefix]")]
        [DefaultValue("")]
        public string Prefix { get; init; } = "";

        [Description("Comma-seperated list of directories to exclude")]
        [CommandOption("--exclude")]
        [DefaultValue("")]
        public string Exclude { get; init; } = "";
    }
    
    private readonly IGitService _gitService;

    public TagsCommand(IGitService gitService)
    {
        _gitService = gitService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        Settings settings)
    {
        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn("Branch");
        table.AddColumn("Version");

        await AnsiConsole.Status().StartAsync("Searching Tags...", async ctx =>
        {
            ctx.Refresh();

            var excludeDirectories = settings.Exclude.Split(',');
            var directories = _gitService
                .GetGitDirectories(settings.Path)
                .Where(d => !excludeDirectories.Contains(d.Name));

            await Parallel.ForEachAsync(directories, async (d, _) =>
            {
                if (settings.Fetch)
                {
                    var runResult = await _gitService.RunAsync("fetch", settings, d);
                    if (runResult.Status == RunStatus.Error)
                    {
                        table.AddRow(d.Name, runResult.Message);
                        return;
                    }
                }

                var branchTask = _gitService.GetCurrentBranchAsync(d);
                var tagTask = _gitService.GetTagAsync(settings.Prefix, d);
                
                table.AddRow(d.Name, (await branchTask)?.Trim('\n') ?? "-", (await tagTask)?.Trim('\n') ?? "-");
            });
            
            AnsiConsole.Write(table);
        });
        
        return 0;
    }
}