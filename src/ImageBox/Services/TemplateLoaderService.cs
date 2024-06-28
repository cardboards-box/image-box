using System.IO.Compression;

namespace ImageBox.Services;

using Ast;
using ImageBox;

/// <summary>
/// Service for loading <see cref="BoxedImage"/> from a file
/// </summary>
public interface ITemplateLoaderService
{
    /// <summary>
    /// Loads the <see cref="BoxedImage"/> from the given path
    /// </summary>
    /// <param name="path">The path to load</param>
    /// <param name="config">The config for the AST parser</param>
    /// <returns>The loaded <see cref="BoxedImage"/></returns>
    /// <exception cref="FileNotFoundException">Thrown if the file could not be found</exception>
    /// <exception cref="InvalidOperationException">Thrown if the loaded boxed image is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if the file does not exist after downloading</exception>
    /// <exception cref="InvalidOperationException">Thrown if the zip file contains another zip file</exception>
    /// <exception cref="InvalidOperationException">Thrown if a module with the same name is already loaded</exception>
    Task<BoxedImage> Load(IOPath path, AstConfig? config = null);
}

internal class TemplateLoaderService(
    IFileResolverService _resolver,
    IAstParserService _parser,
    IElementReflectionService _elements,
    ILogger<TemplateLoaderService> _logger) : ITemplateLoaderService
{
    /// <summary>
    /// Loads the <see cref="BoxedImage"/> from the given path
    /// </summary>
    /// <param name="path">The path to load</param>
    /// <param name="config">The config for the AST parser</param>
    /// <returns>The loaded <see cref="BoxedImage"/></returns>
    /// <exception cref="FileNotFoundException">Thrown if the file could not be found</exception>
    /// <exception cref="InvalidOperationException">Thrown if the loaded boxed image is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if the file does not exist after downloading</exception>
    /// <exception cref="InvalidOperationException">Thrown if the zip file contains another zip file</exception>
    /// <exception cref="InvalidOperationException">Thrown if a module with the same name is already loaded</exception>
    public Task<BoxedImage> Load(IOPath path, AstConfig? config = null)
    {
        config ??= AstConfig.Default;
        return path.Local
            ? Local(path, config)
            : Remote(path, config);
    }

    /// <summary>
    /// Loads the boxed image from the given path
    /// </summary>
    /// <param name="path">The path to load from</param>
    /// <param name="config">The config for the AST parser</param>
    /// <returns>The loaded boxed image</returns>
    public async Task<BoxedImage> Local(IOPath path, AstConfig config)
    {
        //Determine the entry point of the file
        var (filePath, type) = DetermineEntryPoint(path.OSSafe);
        //If the file is a zip, load it
        if (type == EntryPointType.Zip)
            return await Zip(path.OSSafe, config);
        //Get absolute working directory
        var wrkDir = Path.GetDirectoryName(Path.GetFullPath(filePath))!;
        //Get entry point file name
        var entryName = Path.GetFileName(filePath)!;
        //Load all of the elements from the file
        var elements = _parser.ParseFile(filePath, config).ToArray();
        //Load all of the templates from the elements
        var templates = _elements.BindTemplates(elements, config.ThrowErrorsOnBind).ToArray();
        //Create the boxed image instance
        return new BoxedImage
        {
            WorkingDirectory = wrkDir,
            FileName = entryName,
            SyntaxTree = elements,
            Elements = templates
        };
    }

    /// <summary>
    /// Loads a boxed image from a remote source
    /// </summary>
    /// <param name="path">The remote source</param>
    /// <param name="config">The config for the AST parser</param>
    /// <returns>The loaded boxed image</returns>
    /// <exception cref="InvalidOperationException">Thrown if the file does not exist after downloading</exception>
    public async Task<BoxedImage> Remote(IOPath path, AstConfig config)
    {
        //Load the file from the end point
        var (stream, file, type) = await _resolver.Fetch(path);
        //Get the extension from the mime-type
        var ext = IOPathHelper.DetermineExtension(type);
        //Get the file name or generate a random one
        var fileName = string.IsNullOrEmpty(file)
            ? $"{Path.GetRandomFileName()}.{ext}" : file;
        //Get a random directory to store the file in
        var dir = IOPathHelper.RandomDirectory();
        try
        {
            //Save the file to the directory
            var outputPath = Path.Combine(dir, fileName);
            using (var fileStream = File.Create(outputPath))
            {
                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
            //Shouldn't happen... but just in case
            if (!File.Exists(outputPath))
                throw new InvalidOperationException("Failed to save file - Invalid path?");
            //Load the boxed image from the file
            var output = await Local(outputPath, config);
            //Add the directory to the clean up list
            output.Cleanup.Add(dir);
            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load boxed image - Remote file");
            //Delete the directory if it exists - Clean up after ourselves, it's only polite.
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            throw;
        }
    }

    /// <summary>
    /// Loads a boxed image from a zip file
    /// </summary>
    /// <param name="path">The path the zip file is present in</param>
    /// <param name="config">The config for the AST parser</param>
    /// <returns>The loaded boxed image</returns>
    /// <exception cref="InvalidOperationException">Thrown if the zip file contains another zip file</exception>
    public async Task<BoxedImage> Zip(IOPath path, AstConfig config)
    {
        //Create a random directory to store the extracted files
        var dir = IOPathHelper.RandomDirectory();
        try
        {
            //Extract the zip file
            ZipFile.ExtractToDirectory(path, dir);
            //Get the entry point file path
            var (file, type) = DetermineEntryPoint(dir);
            //If it's a zip file, throw an exception because zip-ception
            if (type == EntryPointType.Zip)
                throw new InvalidOperationException("Failed to extract zip file - Why the zip-ception?");
            //Load the boxed image from the entry point file
            var card = await Local(file, config);
            //Add the directory to the cleanup list
            card.Cleanup.Add(dir);
            return card;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load card set - Zip file");
            //Delete the directory if it exists - Clean up after ourselves, it's only polite.
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            throw;
        }
    }

    /// <summary>
    /// Checks to see if the path is a file, directory, or zip file
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>The file's path and the type of resolver</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file could not be found</exception>
    public (string filePath, EntryPointType type) DetermineEntryPoint(string path)
    {
        //Check if the file exists
        if (File.Exists(path))
        {
            //Get the extension of the file
            var ext = Path.GetExtension(path).ToLower().Trim('.');
            //Determine type of file based on extension            
            return ext == "zip"
                ? (path, EntryPointType.Zip)
                : (path, EntryPointType.File);
        }
        //All of the extensions that could possibly be a template
        //These are ordered by priority
        string[] extensions = ["bi", "boxed", "card", "template", "html"];
        //Iterate over each extension and check if the file exists
        foreach (var ext in extensions)
        {
            //Get all of the files with the extension
            var files = Directory.GetFiles(path, $"*.{ext}");
            if (files.Length > 0)
                return (files.First(), EntryPointType.Directory);
        }
        //Couldn't find any matching files
        throw new FileNotFoundException($"No entry point found. Searched extensions: {string.Join(", ", extensions)}", path);
    }

    /// <summary>
    /// Indicates the type of entry point for the boxed images
    /// </summary>
    public enum EntryPointType
    {
        /// <summary>
        /// Entry point is a JSON file 
        /// </summary>
        File = 1,
        /// <summary>
        /// Entry point is a directory
        /// </summary>
        Directory = 2,
        /// <summary>
        /// Entry point has a .zip extension
        /// </summary>
        Zip = 3,
    }
}
