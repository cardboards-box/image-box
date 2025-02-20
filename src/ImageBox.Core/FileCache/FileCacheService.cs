using Path = System.IO.Path;

namespace ImageBox.Core.FileCache;

/// <summary>
/// A service for downloading and caching files from remote sources
/// </summary>
public interface IFileCacheService
{
    /// <summary>
    /// Attempts to get an item from the cache
    /// </summary>
    /// <param name="uri">The URI of the cached item</param>
    /// <returns>The cached file or null if the cache doesn't exist</returns>
    Task<FileResult?> GetCache(string uri);

    /// <summary>
    /// Sets the value of the cache
    /// </summary>
    /// <param name="uri">The URI of the cached item</param>
    /// <param name="result">The file to set as the result</param>
    /// <returns>The cached file result</returns>
    Task<FileResult> SetCache(string uri, FileResult result);
}

internal class FileCacheService(
    IServiceConfig _config,
    IJsonService _json) : IFileCacheService
{
    public async Task<FileResult?> GetCache(string uri)
    {
        var dir = _config.Requests.CacheDirectory;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var hash = uri.MD5Hash();
        var cacheInfo = await ReadCacheInfo(hash, dir);
        if (cacheInfo == null) return null;

        return new(ReadFile(hash, dir), cacheInfo.Name, cacheInfo.MimeType, false);
    }

    public async Task<FileResult> SetCache(string uri, FileResult result)
    {
        var dir = _config.Requests.CacheDirectory;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var hash = uri.MD5Hash();

        var io = new MemoryStream();
        await result.Stream.CopyToAsync(io);
        await result.Stream.FlushAsync();
        await result.Stream.DisposeAsync();

        io.Position = 0;
        var cacheInfo = new FileCacheItem(result.FileName, result.MimeType, DateTime.Now);
        var worked = await WriteFile(io, hash, dir);
        if (worked)
            await WriteCacheInfo(hash, cacheInfo, dir);

        io.Position = 0;
        return new(io, result.FileName, result.MimeType, false);
    }

    /// <summary>
    /// Gets the path of the cached file from the given cache directory and hash
    /// </summary>
    /// <param name="hash">The file hash</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>The formatted file path</returns>
    public static string FilePath(string hash, string cacheDir) => Path.Combine(cacheDir, $"{hash}.data");

    /// <summary>
    /// Gets the path of the cache metadata file from the given cache directory and hash
    /// </summary>
    /// <param name="hash">The file hash</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>The formatted file path</returns>
    public static string CachePath(string hash, string cacheDir) => Path.Combine(cacheDir, $"{hash}.cache.json");

    /// <summary>
    /// Reads the given file from the disk
    /// </summary>
    /// <param name="hash">The file hash</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>The file system stream</returns>
    public static Stream ReadFile(string hash, string cacheDir)
    {
        var path = FilePath(hash, cacheDir);
        return File.OpenRead(path);
    }

    /// <summary>
    /// Writes the cached file to the file system
    /// </summary>
    /// <param name="stream">The stream to write</param>
    /// <param name="hash">The file hash</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>Whether or not the stream was written correctly</returns>
    public static async Task<bool> WriteFile(Stream stream, string hash, string cacheDir)
    {
        try
        {
            var path = FilePath(hash, cacheDir);
            using var io = File.Create(path);
            await stream.CopyToAsync(io);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Reads the cache metadata from the file system
    /// </summary>
    /// <param name="hash">The file hash</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>The cache metadata or null if it wasn't found</returns>
    public async Task<FileCacheItem?> ReadCacheInfo(string hash, string cacheDir)
    {
        var path = CachePath(hash, cacheDir);
        if (!File.Exists(path)) return null;

        using var io = File.OpenRead(path);
        return await _json.Deserialize<FileCacheItem>(io);
    }

    /// <summary>
    /// Writes the given file cache information to the file system
    /// </summary>
    /// <param name="hash">The file hash</param>
    /// <param name="item">The file cache metadata to write</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <returns>A task representing the completion of writing the file cache to the file system</returns>
    public async Task WriteCacheInfo(string hash, FileCacheItem item, string cacheDir)
    {
        try
        {
            var path = CachePath(hash, cacheDir);
            using var io = File.Create(path);
            await _json.Serialize(item, io);
        }
        catch { }
    }
}
