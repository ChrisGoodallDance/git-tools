namespace git_tools.Services;

public interface IFileService
{
    Task<string?> ReadFileAsync(string filename, DirectoryInfo directory, bool recursive,
        CancellationToken token = default);
}

internal sealed class FileService : IFileService
{
    public async Task<string?> ReadFileAsync(string filename, DirectoryInfo directory, bool recursive, CancellationToken token = default)
    {
        var path = Path.Join(directory.FullName, filename);
        if (File.Exists(path))
        {
            return await File.ReadAllTextAsync(path, token);
        }

        if (recursive)
        {
            foreach (var d in directory.GetDirectories().OrderBy(d => d.Name))
            {
                var value = await ReadFileAsync(filename, d, true, token);
                if (value != null)
                    return value;
            }
        }
        
        return null;
    }
}