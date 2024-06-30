namespace ImageBox.Core.Config;

/// <summary>
/// The configuration for the application
/// </summary>
public class ImageBoxConfig : IImageBoxConfig
{
    /// <summary>
    /// The cache directory for the file requests
    /// </summary>
    public string CacheDirectory { get; set; } = ImageBoxInternalConfig.DefaultCacheDir;

    /// <summary>
    /// The user agent to use for the file requests
    /// </summary>
    public string UserAgent { get; set; } = ImageBoxInternalConfig.DefaultUserAgent;

    /// <summary>
    /// Configure the default http request for file caching
    /// </summary>
    public Action<HttpRequestMessage>? CacheRequestConfig { get; set; }

    /// <summary>
    /// The timeout for script executions
    /// </summary>
    public TimeUnit ScriptTimeout { get; set; } = ImageBoxInternalConfig.DefaultScriptTimeout;

    /// <summary>
    /// The maximum recursion limit for scripts
    /// </summary>
    public int ScriptRecursionLimit { get; set; } = ImageBoxInternalConfig.DefaultScriptRecursionLimit;

    /// <summary>
    /// The memory limit for scripts (in megabytes)
    /// </summary>
    public double ScriptMemoryLimitMb { get; set; } = ImageBoxInternalConfig.DefaultScriptMemoryLimitMb;

    /// <summary>
    /// The default font size for rendered images
    /// </summary>
    public SizeUnit FontSize { get; set; } = ImageBoxInternalConfig.DefaultFontSize;

    /// <summary>
    /// The default FPS for the animations
    /// </summary>
    public double AnimateFps { get; set; } = ImageBoxInternalConfig.DefaultAnimateFps;

    /// <summary>
    /// How many times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    public ushort AnimateRepeat { get; set; } = ImageBoxInternalConfig.DefaultAnimateRepeat;
}
