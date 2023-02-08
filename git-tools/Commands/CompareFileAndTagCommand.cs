namespace git_tools.Commands;

[Description("Compare contents of a file (BuildNumber.txt) with a tag")]
internal sealed class CompareFileAndTagCommand : AsyncCommand<CompareFileAndTagCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<CompareFileAndTagCommand>("compare-tag")
            .WithExample(new [] { "compare-tag", "BuildNumber.txt", "--tag-prefix", "release" });
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [Description("File to compare")]
        [CommandArgument(0, "[name]")]
        [DefaultValue("BuildNumber.txt")]
        public string Name { get; init; } = "BuildNumber.txt";

        [CommandOption("--tag-prefix")]
        [Description("The tag prefix")]
        [DefaultValue("release")]
        public string TagPrefix { get; init; } = "release";
    }
    
    private readonly IFileService _fileService;
    private readonly IGitService _gitService;

    public CompareFileAndTagCommand(IGitService gitService, IFileService fileService)
    {
        _gitService = gitService;
        _fileService = fileService;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        Settings settings)
    {
        var table = new Table();
        table.AddColumn("Component");
        table.AddColumn(settings.Name);
        table.AddColumn("Tag");

        var directories = _gitService.GetGitDirectories(settings.Path);

        await Parallel.ForEachAsync(directories, async (directory, token) =>
        {
            var fileContents = await _fileService.ReadFileAsync(settings.Name, directory, true, token);
            var tag = (await _gitService.GetTagAsync(settings.TagPrefix, directory))?.TrimEnd('\n');
            var colour = (tag ?? "").Contains(fileContents ?? "INVALID") ? "green" : "red";
            
            table.AddRow(
                new Markup(directory.Name),
                new Markup($"[{colour}]{fileContents ?? "-"}[/]"),
                new Markup($"[{colour}]{tag ?? "-"}[/]")
            );
        });

        AnsiConsole.Write(table);

        return 0;
    }

    
}