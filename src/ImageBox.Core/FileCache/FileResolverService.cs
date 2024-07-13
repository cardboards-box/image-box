namespace ImageBox.Core.FileCache;

using IOPath;

/// <summary>
/// A utility for fetching files from various sources
/// </summary>
public interface IFileResolverService
{
    /// <summary>
    /// Fetches a file from the given path
    /// </summary>
    /// <param name="path">The path to fetch the file from</param>
    /// <returns>The file results</returns>
    Task<FileResult> Fetch(IOPath path);
}

internal class FileResolverService(
    IFileCacheService _cache) : IFileResolverService
{
    public Task<FileResult> Fetch(IOPath path)
    {
        if (path.Type.HasFlag(IOPathType.Cache))
            throw new NotSupportedException("Cache paths are not supported via raw-file resolution as it requires the cache context");

        if (path.Type.HasFlag(IOPathType.Http))
            return GetHttp(path.OSSafe);

        if (path.Type.HasFlag(IOPathType.Ftp))
            return GetFtp(path.OSSafe);

        if (path.Type.HasFlag(IOPathType.Local))
            return GetLocal(path.OSSafe);

        throw new NotSupportedException($"The path type {path.Type} is not supported");
    }

    public async Task<FileResult> GetHttp(string url)
    {
        var (stream, fileName, mimeType) = await _cache.GetFile(url);
        return new(stream, fileName, mimeType);
    }

    public static Task<FileResult> GetLocal(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The file path does not exist", path);

        var mimeType = MimeTypes.GetMimeType(path);
        var stream = File.OpenRead(path);
        var name = Path.GetFileName(path);
        return Task.FromResult(new FileResult(stream, name, mimeType));
    }

    public Task<FileResult> GetFtp(string url)
    {
        throw new NotImplementedException("FTP is not supported yet");
    }
}

/// <summary>
/// Represents the result of a file request
/// </summary>
/// <param name="Stream">The stream to read the file contents from</param>
/// <param name="FileName">The name of the file </param>
/// <param name="MimeType"></param>
public record class FileResult(
    Stream Stream,
    string FileName,
    string MimeType);