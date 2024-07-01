namespace ImageBox.Core.Config;

/// <summary>
/// The default configuration for any boxed image services
/// </summary>
/// <param name="_config"></param>
public class ImageBoxInternalConfig(
    IConfiguration _config) : IImageBoxConfig
{
    private string? _cacheDir;
    private string? _userAgent;
    private TimeUnit? _scriptTimeout;
    private int? _scriptRecursionLimit;
    private double? _scriptMemoryLimitMb;
    private SizeUnit? _fontSize;
    private double? _fps;
    private ushort? _repeat;

    /// <summary>
    /// The default section for the configuration
    /// </summary>
    public const string SECTION = "BoxedImage";

    #region Default Values
    /// <summary>
    /// The default file cache directory
    /// </summary>
    public static string DefaultCacheDir { get; set; } = "cache";

    /// <summary>
    /// The default user-agent for downloading files
    /// </summary>
    public static string DefaultUserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36";

    /// <summary>
    /// The default timeout for script executions
    /// </summary>
    public static TimeUnit DefaultScriptTimeout { get; set; } = TimeUnit.Parse("10s");

    /// <summary>
    /// The default recursion limit for scripts
    /// </summary>
    public static int DefaultScriptRecursionLimit { get; set; } = 900;

    /// <summary>
    /// The default memory limit for scripts (in megabytes)
    /// </summary>
    public static double DefaultScriptMemoryLimitMb { get; set; } = 4;

    /// <summary>
    /// The default font size for the images
    /// </summary>
    public static SizeUnit DefaultFontSize { get; set; } = SizeUnit.Parse("16px");

    /// <summary>
    /// The default FPS for the animations
    /// </summary>
    public static double DefaultAnimateFps { get; set; } = 15;

    /// <summary>
    /// The default number of times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    public static ushort DefaultAnimateRepeat { get; set; } = 1;
    #endregion

    /// <summary>
    /// The section of the configuration file for boxed image services
    /// </summary>
    public IConfigurationSection Section => _config.GetSection(SECTION);

    /// <summary>
    /// The cache directory for the file requests
    /// </summary>
    public string CacheDirectory => _cacheDir ??= Section["CacheDirectory"] ?? DefaultCacheDir;

    /// <summary>
    /// The user agent to use for the file requests
    /// </summary>
    public string UserAgent => _userAgent ??= Section["UserAgent"] ?? DefaultUserAgent;

    /// <summary>
    /// The timeout for script executions
    /// </summary>
    public TimeUnit ScriptTimeout => _scriptTimeout ??= Section["ScriptTimeout"] ?? DefaultScriptTimeout;

    /// <summary>
    /// The maximum recursion limit for scripts
    /// </summary>
    public int ScriptRecursionLimit => _scriptRecursionLimit ??=
        int.TryParse(Section["ScriptRecursionLimit"], out var value)
        ? value : DefaultScriptRecursionLimit;

    /// <summary>
    /// The memory limit for scripts (in megabytes)
    /// </summary>
    public double ScriptMemoryLimitMb => _scriptMemoryLimitMb ??=
        double.TryParse(Section["ScriptMemoryLimit"], out var value)
        ? value : DefaultScriptMemoryLimitMb;

    /// <summary>
    /// The default font size for rendered images
    /// </summary>
    public SizeUnit FontSize => _fontSize ??= Section["FontSize"] ?? DefaultFontSize;

    /// <summary>
    /// The default FPS for the animations
    /// </summary>
    public double AnimateFps => _fps ??=
        double.TryParse(Section["AnimateFps"], out var value)
        ? value : DefaultAnimateFps;

    /// <summary>
    /// How many times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    public ushort AnimateRepeat => _repeat ??=
        ushort.TryParse(Section["AnimateRepeat"], out var value)
        ? value : DefaultAnimateRepeat;

    /// <summary>
    /// Configure the default http request for file caching
    /// </summary>
    public Action<HttpRequestMessage>? CacheRequestConfig { get; set; }
}
