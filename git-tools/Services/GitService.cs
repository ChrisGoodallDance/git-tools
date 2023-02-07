using System.Text.RegularExpressions;

namespace git_tools.Services;

public interface IGitService
{
    IEnumerable<DirectoryInfo> GetGitDirectories(string path);
    IEnumerable<DirectoryInfo> GetGitDirectories(DirectoryInfo directoryInfo);
    Task<RunResult> RunAsync(string command, GlobalSettings settings, DirectoryInfo di);
    Task<string?> GetProjectUrlAsync(DirectoryInfo directory);
    Task<string?> GetTagAsync(string prefix, DirectoryInfo directory);
    Task<string?> GetCurrentBranchAsync(DirectoryInfo directory);
}

internal sealed class GitService : IGitService
{
    private readonly IProcessService _processService;

    public GitService(IProcessService processService)
    {
        _processService = processService;
    }

    public IEnumerable<DirectoryInfo> GetGitDirectories(string path)
    {
        path = path.Replace("~", Environment.GetEnvironmentVariable("HOME"));
        return GetGitDirectories(new DirectoryInfo(path));
    }

    public IEnumerable<DirectoryInfo> GetGitDirectories(DirectoryInfo directoryInfo)
    {
        var directories = new List<DirectoryInfo>();
        
        foreach (var directory in directoryInfo.EnumerateDirectories())
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")))
            {
                directories.Add(directory);
            }
            else
            {
                directories.AddRange(GetGitDirectories(directory));
            }
        }

        return directories.OrderBy(d => d.FullName);
    }

    public async Task<RunResult> RunAsync(string command, GlobalSettings settings, DirectoryInfo di)
    {
        AnsiConsole.WriteLine("Performing " + command + " on " + di.FullName);
        return await RunAsync(command, di);
    }

    public async Task<string?> GetProjectUrlAsync(DirectoryInfo directory)
    {
        var match = new Regex("git@([\\w\\-\\.]+:[\\w\\-\\/]+).git").Match(await GetContentsOfGitConfigAsync(directory));
        return !match.Success || match.Groups.Count == 0
            ? null
            : "https://" + match.Groups[1].Captures.FirstOrDefault()?.Value.Replace(':', '/');
    }

    public async Task<string?> GetTagAsync(string prefix, DirectoryInfo directory)
    {
        //var command = "tag --list '" + prefix + "*' --sort=v:refname | tail -1";
        var command = "describe --tags";
        var runResult = await _processService.RunAsync($"git -C \"{directory.FullName}\" {command}", directory);
        
        return runResult.Status == RunStatus.Success ? runResult.Message : null;
    }

    public async Task<string?> GetCurrentBranchAsync(DirectoryInfo directory)
    {
        var command = "rev-parse --abbrev-ref HEAD";
        var runResult = await _processService.RunAsync($"git -C \"{directory.FullName}\" {command} -q", directory);
        
        return runResult.Status == RunStatus.Success ? runResult.Message : null;
    }

    private async Task<RunResult> RunAsync(string command, DirectoryInfo di)
    {
        return await _processService.RunAsync($"git -C \"{di.FullName}\" {command} -q", di);
    }

    private static async Task<string> GetContentsOfGitConfigAsync(DirectoryInfo directory)
    {
        return await File.ReadAllTextAsync(Path.Combine(directory.FullName, ".git", "config"));
    }
}