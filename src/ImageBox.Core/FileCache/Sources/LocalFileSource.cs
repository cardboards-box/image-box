namespace ImageBox.Core.FileCache.Sources;

internal class LocalFileSource : IFileSourceService
{
    public static IEnumerable<string> DeterminePaths(FileFetchProperties properties)
    {
        yield return properties.Source.OSSafe;
        yield return properties.Source.GetAbsolute(properties.WorkingDirectory).OSSafe;
        yield return Path.Combine(properties.WorkingDirectory ?? string.Empty, properties.Source.OSSafe);
    }

    public Task<FileResult?> Fetch(FileFetchProperties properties)
    {
        if (!properties.Source.Type.HasFlag(IOPathType.Local))
            return Task.FromResult<FileResult?>(null);

        var path = DeterminePaths(properties).FirstOrDefault(File.Exists);
        if (string.IsNullOrEmpty(path)) 
            return Task.FromResult<FileResult?>(null);

        var mimeType = MimeTypes.GetMimeType(path);
        var stream = File.OpenRead(path);
        var name = Path.GetFileName(path);
        var result = new FileResult(stream, name, mimeType, false);
        return Task.FromResult<FileResult?>(result);
    }
}
