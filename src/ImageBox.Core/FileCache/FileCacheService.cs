namespace ImageBox.Core.FileCache;

/// <summary>
/// A service for downloading and caching files from remote sources
/// </summary>
public interface IFileCacheService
{
    /// <summary>
    /// Gets the file either from the cache or from the given URL
    /// </summary>
    /// <param name="url">The url to fetch</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <param name="userAgent">The user agent to use for the file download</param>
    /// <returns>A task representing the results of the cached file request</returns>
    Task<FileCacheResult> GetFile(
        string url,
        string? cacheDir = null, string? userAgent = null);
}

internal class FileCacheService(
    IServiceConfig _config,
    IApiService _api,
    IJsonService _json) : IFileCacheService
{
    public async Task<FileCacheResult> GetData(string url,
        string? userAgent = null)
    {
        userAgent ??= _config.Requests.UserAgent;
        var req = await _api.Create(url)
            .Accept("*/*")
            .With(c =>
            {
                c.Headers.Add("user-agent", userAgent);
                _config.Requests.Configure?.Invoke(c);
            })
            .Result() ?? throw new NullReferenceException($"Request returned null for: {url}");
        req.EnsureSuccessStatusCode();

        var headers = req.Content.Headers;
        var path = headers?.ContentDisposition?.FileName ?? headers?.ContentDisposition?.Parameters?.FirstOrDefault()?.Value ?? "";
        var type = headers?.ContentType?.ToString() ?? "";

        return new(await req.Content.ReadAsStreamAsync(), path, type);
    }

    /// <summary>
    /// Gets the file either from the cache or from the given URL
    /// </summary>
    /// <param name="url">The url to fetch</param>
    /// <param name="cacheDir">The cache directory</param>
    /// <param name="userAgent">The user agent to use for the file download</param>
    /// <returns>A task representing the results of the cached file request</returns>
    public async Task<FileCacheResult> GetFile(
        string url,
        string? cacheDir = null, string? userAgent = null)
    {
        cacheDir ??= _config.Requests.CacheDirectory;

        if (!Directory.Exists(cacheDir))
            Directory.CreateDirectory(cacheDir);

        var hash = url.MD5Hash();

        var cacheInfo = await ReadCacheInfo(hash, cacheDir);
        if (cacheInfo != null)
            return new(ReadFile(hash, cacheDir), cacheInfo.Name, cacheInfo.MimeType);

        var io = new MemoryStream();
        var (stream, file, type) = await GetData(url, userAgent);
        await stream.CopyToAsync(io);
        io.Position = 0;
        cacheInfo = new FileCacheItem(file, type, DateTime.Now);
        var worked = await WriteFile(io, hash, cacheDir);
        if (worked)
            await WriteCacheInfo(hash, cacheInfo, cacheDir);
        io.Position = 0;

        return new(io, file, type);
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
