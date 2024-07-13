namespace ImageBox.Core.Config;


/// <summary>
/// The configuration for the script engine
/// </summary>
public class ScriptConfig
{
    /// <summary>
    /// Configuration option bind for <see cref="TimeoutUnit"/>
    /// </summary>
    public string Timeout
    {
        get => TimeoutUnit;
        set => TimeoutUnit = value;
    }

    /// <summary>
    /// The timeout for script executions
    /// </summary>
    /// <value>Default value is 10 seconds, default config path is `ImageBox:Scripts:TimeoutUnit`.</value>
    public TimeUnit TimeoutUnit { get; set; } = "10s";

    /// <summary>
    /// The max number of recursive calls for the scripts
    /// </summary>
    /// <value>Default value is 900, default config path is `ImageBox:Scripts:RecursionLimit`.</value>
    public int RecursionLimit { get; set; } = 900;

    /// <summary>
    /// The maximum amount of memory a script can use
    /// </summary>
    /// <value>Default value is 4MB, default config path is `ImageBox:Scripts:MemoryLimitMb`</value>
    public double MemoryLimitMb { get; set; } = 4;
}
