namespace git_tools.Commands;

[DisplayName("branch")]
[Description("Show the current checked out branch")]
internal sealed class BranchCommmand : PerformCommand
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<BranchCommmand>("branch");
    }
    
    public BranchCommmand(IGitService gitService)
        : base(_ => "rev-parse --abbrev-ref HEAD", gitService)
    {
    }
}