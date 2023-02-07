namespace git_tools.Commands;

[Description("Perform a pull")]
internal sealed class PullCommand : PerformCommand
{
    public PullCommand(IGitService gitService)
        : base(_ => "pull", gitService)
    {
    }
}