using System.Diagnostics;
using System.Runtime.InteropServices;

namespace git_tools.Services;

public interface IProcessService
{
    Task<RunResult> RunAsync(string command, DirectoryInfo directory);
    void OpenUrl(string url);
}

internal sealed class ProcessService : IProcessService
{
    public async Task<RunResult> RunAsync(string command, DirectoryInfo directory)
    {
        var str = command.Replace("\"", "\\\"");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + str + "\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode == 0 
            ? RunResult.Success(await process.StandardOutput.ReadToEndAsync(), directory) 
            : RunResult.Error(await process.StandardError.ReadToEndAsync(), directory);
    }

    public void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}