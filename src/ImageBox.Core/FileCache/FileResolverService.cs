namespace ImageBox.Core.FileCache;

using IOPath;

/// <summary>
/// A utility for fetching files from various sources
/// </summary>
public interface IFileResolverService
{
    /// <summary>
    /// Fetches the file from the given properties
    /// </summary>
    /// <param name="path">The path to fetch the file from</param>
    /// <param name="workDir">The working directory of the fetch</param>
    /// <returns>The file results</returns>
    Task<FileResult> Fetch(IOPath path, string? workDir = null);

    /// <summary>
    /// Fetches the file from the given properties
    /// </summary>
    /// <param name="properties">The properties of the file fetch operation</param>
    /// <returns>The file results</returns>
    Task<FileResult> Fetch(FileFetchProperties properties);
}

internal class FileResolverService(
    IEnumerable<IFileSourceService> _sources,
    IEnumerable<IFileTransformerService> _transformers,
    IFileCacheService _cache) : IFileResolverService
{
    public async Task<FileResult> Fetch(FileFetchProperties properties)
    {
        var uri = properties.Source.OSSafe;
        var cached = await _cache.GetCache(uri);
        if (cached is not null) return cached;

        var result = await ResolveFile(properties);
        result = await Transform(result, properties);

        if (result.Cacheable)
            return await _cache.SetCache(uri, result);

        return result;
    }

    public async Task<FileResult> Transform(FileResult result, FileFetchProperties properties)
    {
        foreach (var transformer in _transformers)
            result = await transformer.Transform(result, properties);

        return result;
    }

    public async Task<FileResult> ResolveFile(FileFetchProperties properties)
    {
        var uri = properties.Source.OSSafe;
        foreach (var source in _sources)
        {
            var result = await source.Fetch(properties);
            if (result is not null) return result;
        }

        throw new NotImplementedException($"No source was able to resolve the file: {uri}");
    }

    public Task<FileResult> Fetch(IOPath path, string? workDir = null)
    {
        return Fetch(new FileFetchProperties(path)
        {
            WorkingDirectory = workDir
        });
    }
}