namespace git_tools.Commands;

[Description("Show the contents of a file")]
internal sealed class CatCommand : AsyncCommand<CatCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<CatCommand>("cat");
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [Description("Outputs the contents of the file name")]
        [CommandArgument(0, "[name]")]
        [DefaultValue("BuildNumber.txt")]
        public string Name { get; init; } = "BuildNumber.txt";
    }
    
    private readonly IFileService _fileService;
    private readonly IGitService _gitService;

    public CatCommand(IGitService gitService, IFileService fileService)
    {
        _gitService = gitService;
        _fileService = fileService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn(settings.Name);

        var directories = _gitService
            .GetGitDirectories(settings.Path);
        
        await Parallel.ForEachAsync(directories, async (d, token) =>
        {
            var fileContents = await _fileService.ReadFileAsync(settings.Name, d, true, token);
            table.AddRow(d.Name, fileContents ?? "-");
        });
        
        AnsiConsole.Write(table);
        
        return 0;
    }
}