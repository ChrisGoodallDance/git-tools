namespace git_tools.Commands;

[Description("Perform a fetch")]
internal sealed class FetchCommand : PerformCommand<FetchCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<FetchCommand>("fetch");
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [CommandOption("--prune")]
        [Description("Prune local branches and tags")]
        [DefaultValue(false)]
        public bool Prune { get; init; }
    }
    
    public FetchCommand(IGitService gitService)
        : base(settings => "fetch " + (settings.Prune ? "--prune" : ""), gitService)
    {
    }
}