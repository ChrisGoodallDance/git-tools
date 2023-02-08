namespace git_tools.Commands;

[Description("Open all repositories in web browser")]
internal sealed class LinksCommand : AsyncCommand<LinksCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<LinksCommand>("links")
            .WithExample(new [] { "links", "--open" });
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [Description("Open the repository links in the browser")]
        [CommandOption("--open")]
        [DefaultValue(false)]
        public bool Open { get; init; }
    }
    
    private readonly IGitService _gitService;
    private readonly IProcessService _processService;

    public LinksCommand(IGitService gitService, IProcessService processService)
    {
        _gitService = gitService;
        _processService = processService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn("Link");
        
        var directories = _gitService
            .GetGitDirectories(settings.Path)
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase);

        await Parallel.ForEachAsync(directories, async (directory, token) =>
        {
            var projectUrl = await _gitService.GetProjectUrlAsync(directory);
            table.AddRow(
                new Markup(directory.Name),
                new Markup("[link deepskyblue3_1 underline]" + projectUrl + "[/]")
            );

            if (settings.Open && projectUrl != null)
            {
                _processService.OpenUrl(projectUrl);
            }
        });
        
        AnsiConsole.Write(table);
        
        return 0;
    }
}