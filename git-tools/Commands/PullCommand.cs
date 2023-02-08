namespace git_tools.Commands;

[Description("Perform a pull")]
internal sealed class PullCommand : PerformCommand
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<PullCommand>("pull");
    }
    
    public PullCommand(IGitService gitService)
        : base(_ => "pull", gitService)
    {
    }
}