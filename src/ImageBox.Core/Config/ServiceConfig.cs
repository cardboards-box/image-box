namespace ImageBox.Core.Config;

/// <summary>
/// The configuration to use for the services 
/// </summary>
public interface IServiceConfig
{
    /// <summary>
    /// Whether to only include internal services or third-party ones as well
    /// </summary>
    bool InternalServicesOnly { get; set; }

    /// <summary>
    /// The configuration for parsing AST data
    /// </summary>
    ParserConfig Parser { get; set; }

    /// <summary>
    /// The configuration for rendering images
    /// </summary>
    RenderConfig Render { get; set; }

    /// <summary>
    /// The configuration for HTTP requests
    /// </summary>
    RequestConfig Requests { get; set; }

    /// <summary>
    /// The configuration for scripts 
    /// </summary>
    ScriptConfig Scripts { get; set; }
}

/// <summary>
/// The configuration to use for the services 
/// </summary>
public class ServiceConfig : IServiceConfig
{
    /// <summary>
    /// Whether to only include internal services or third-party ones as well
    /// </summary>
    public bool InternalServicesOnly { get; set; } = false;

    /// <summary>
    /// The configuration for parsing AST data
    /// </summary>
    public ParserConfig Parser { get; set; } = new();

    /// <summary>
    /// The configuration for rendering images
    /// </summary>
    public RenderConfig Render { get; set; } = new();

    /// <summary>
    /// The configuration for HTTP requests
    /// </summary>
    public RequestConfig Requests { get; set; } = new();

    /// <summary>
    /// The configuration for scripts 
    /// </summary>
    public ScriptConfig Scripts { get; set; } = new();
}
