namespace ImageBox.Core.FileCache.Sources;

internal class LocalFileSource : IFileSourceService
{
    public Task<FileResult?> Fetch(FileFetchProperties properties)
    {
        if (!properties.Source.Type.HasFlag(IOPathType.Local))
            return Task.FromResult<FileResult?>(null);

        var path = properties.Source.OSSafe;

        var mimeType = MimeTypes.GetMimeType(path);
        var stream = File.OpenRead(path);
        var name = Path.GetFileName(path);
        var result = new FileResult(stream, name, mimeType, false);
        return Task.FromResult<FileResult?>(result);
    }
}
