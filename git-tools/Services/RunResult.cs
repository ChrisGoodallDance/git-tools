namespace git_tools.Services;

public class RunResult
{
    public string Message { get; }
    public RunStatus Status { get; }
    public DirectoryInfo Directory { get; }
    
    private RunResult(RunStatus status, string message, DirectoryInfo directory) =>
        (Message, Status, Directory) = (message, status, directory);
    
    public static RunResult Error(string message, DirectoryInfo directory)
    {
        return new RunResult(RunStatus.Error, message, directory);
    }

    public static RunResult Success(string message, DirectoryInfo directory)
    {
        return new RunResult(RunStatus.Success, message, directory);
    }
}