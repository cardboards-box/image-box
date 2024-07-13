using System.Collections.Concurrent;
using System.Diagnostics;

namespace ImageBox.Cli.Verbs;

using Drawing;
using Services;

[Verb("watch-directory", false, ["watch-dir", "wd"], HelpText = "Watches a directory for changes and regenerates the image")]
public class WatchDirVerbOptions
{
    [Option('d', "directory", HelpText = "The directory to watch", Required = true)]
    public string? Directory { get; set; }

    [Option('s', "subdirs", HelpText = "Whether to watch files in subdirectories or just the parent directory", Default = true)]
    public bool SubDirs { get; set; } = true;

    [Option('f', "filters", HelpText = "What file extensions to watch for changes (comma separated)")]
    public string Filters { get; set; } = "*.ib,*.card,*.face,*.temp,*.template";
}

internal class WatchDirVerb(
    ILogger<WatchDirVerb> logger,
    IImageBoxService _image) : BooleanVerb<WatchDirVerbOptions>(logger)
{
    private readonly ConcurrentBag<string> _activeFiles = [];
    private readonly ConcurrentBag<string> _requeue = [];

    public async Task Render(string path)
    {
        try
        {
            //Make sure the file is not already being rendered
            if (_activeFiles.Contains(path))
            {
                _logger.LogWarning("File is already being rendered, setting up for requeue: {path}", path);
                //Requeue the file to be rendered again
                if (!_requeue.Contains(path)) 
                    _requeue.Add(path);
                return;
            }
            //Add the file to the active files list
            _activeFiles.Add(path);
            //Start a stop-watch to time the rendering process
            var watch = Stopwatch.StartNew();
            _logger.LogInformation("File change detected, rendering image: {path}", path);
            //Get the image from the cache (or create a new one)
            var im = _image.Create(path);
            //Load the context from the image
            var ctx = await _image.LoadContext(im);
            //Get the full path of the file
            var full = Path.GetFullPath(path);
            //Get the filename without the extension
            var filename = Path.GetFileNameWithoutExtension(full);
            //Determine the extension based on the context
            var ext = ctx.Animate ? "gif" : "png";
            //Get the directory of the file
            var dir = Path.GetDirectoryName(full)!;
            //Create the output path for the rendered image
            var output = Path.Combine(dir, $"{filename}.{ext}");
            _logger.LogInformation("Starting render for {path} >> {output}", full, output);
            //Render the image
            await _image.RenderToFile(output, im);
            //Stop the stop-watch
            watch.Stop();
            _logger.LogInformation("Image rendered in {time}ms >> {output}", watch.ElapsedMilliseconds, output);
            //Remove the file from the active files list
            _activeFiles.TryTake(out _);
            //Check to see if the file needs to be requeued
            if (!_requeue.Contains(path)) return;
            //Requeue the file for regeneration
            _logger.LogInformation("Requeuing file for rendering: {path}", path);
            _requeue.TryTake(out _);
            await Render(path);
        }
        catch (RenderContextException ex)
        {
            _logger.LogError(ex, "Error occurred while rendering image: {path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rendering image: {path}", path);
        }
    }

    public override async Task<bool> Execute(WatchDirVerbOptions options, CancellationToken token)
    {
        //Create the function to run the render task on a new thread
        var run = (string path) =>
        {
            //Create the task with the current token
            _ = Task.Run(() => Render(path), token);
        };

        //Validate the directory
        if (string.IsNullOrWhiteSpace(options.Directory))
        {
            _logger.LogError("No directory was provided to watch");
            return false;
        }
        if (!Directory.Exists(options.Directory))
        {
            _logger.LogError("Directory does not exist: {Directory}", options.Directory);
            return false;
        }
        //Create a debounced version of the run function (to avoid multiple calls from the watcher)
        var bouncy = run.Debounce(100);
        //Get all of the filters to apply to the watcher
        var filters = options.Filters?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim()).ToArray();
        if (filters is null || filters.Length == 0)
            filters = ["*.*"];
        //Create the watcher
        using var watcher = new FileSystemWatcher();
        //Attach the debounce function to the watcher
        watcher.Changed += (sender, e) => bouncy(e.FullPath);
        //Set the directory to watch
        watcher.Path = options.Directory;
        //Only care about when the file changes
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        //Whether or not to watch subdirectories
        watcher.IncludeSubdirectories = options.SubDirs;
        //Apply all of the filters
        foreach (var filter in filters)
            watcher.Filters.Add(filter);
        //Start the watcher
        watcher.EnableRaisingEvents = true;
        //Stop the verb from finishing until sig-term is received
        await Task.Delay(-1, token);
        return true;
    }
}
