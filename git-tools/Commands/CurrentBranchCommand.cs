using git_tools.Services;

namespace git_tools.Commands;

[Description("Show the current checked out branch")]
internal sealed class CurrentBranchCommand : PerformCommand
{
    public CurrentBranchCommand(IGitService gitService)
        : base(_ => "rev-parse --abbrev-ref HEAD", gitService)
    {
    }
}