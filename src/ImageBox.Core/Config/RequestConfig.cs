namespace ImageBox.Core.Config;

using IOPath;

/// <summary>
/// The configuration for HTTP requests within the library
/// </summary>
public class RequestConfig
{
    internal const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36";

    /// <summary>
    /// Internal configuration bind option for <see cref="CacheDirectory"/>
    /// </summary>
    public string Cache
    {
        get => CacheDirectory;
        set => CacheDirectory = value;
    }

    /// <summary>
    /// The directory to store cached files in
    /// </summary>
    /// <value>Default value is `cache`, default config path is `ImageBox:Requests:Cache`.</value>
    public IOPath CacheDirectory { get; set; } = "cache";

    /// <summary>
    /// The user agent to use for requests
    /// </summary>
    /// <value>Default value is a fake user agent, default config path is `ImageBox:Requests:UserAgent`.</value>
    public string UserAgent { get; set; } = USER_AGENT;

    /// <summary>
    /// Configure the HTTP headers and message for requests
    /// </summary>
    /// <value>Not bound to default configuration values</value>
    public Action<HttpRequestMessage>? Configure { get; set; }
}
