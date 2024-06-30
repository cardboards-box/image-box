namespace ImageBox.Core.Config;

/// <summary>
/// The configuration for any boxed image services
/// </summary>
public interface IImageBoxConfig
{
    /// <summary>
    /// The default cache directory for the file requests
    /// </summary>
    string CacheDirectory { get; }

    /// <summary>
    /// The default user agent to use for the file requests
    /// </summary>
    string UserAgent { get; }

    /// <summary>
    /// Configure the default http request for file caching
    /// </summary>
    Action<HttpRequestMessage>? CacheRequestConfig { get; set; }

    /// <summary>
    /// The timeout for script executions
    /// </summary>
    TimeUnit ScriptTimeout { get; }

    /// <summary>
    /// The maximum recursion limit for scripts
    /// </summary>
    int ScriptRecursionLimit { get; }

    /// <summary>
    /// The memory limit for scripts (in megabytes)
    /// </summary>
    double ScriptMemoryLimitMb { get; }

    /// <summary>
    /// The default font size for rendered images
    /// </summary>
    SizeUnit FontSize { get; }

    /// <summary>
    /// The default FPS for the animations
    /// </summary>
    double AnimateFps { get; }

    /// <summary>
    /// How many times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    ushort AnimateRepeat { get; }
}