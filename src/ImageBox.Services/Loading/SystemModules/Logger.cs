namespace ImageBox.Services.Loading.SystemModules;

/// <summary>
/// A service for logging messages within an image script
/// </summary>
/// <param name="_logger"></param>
/// <param name="_ast"></param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
public class Logger(ILogger _logger, LoadedAst _ast) : IScriptItem
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    /// <summary>
    /// Default formatting for logging messages
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>The formatted message</returns>
    public string GenerateMessage(string message)
    {
        return $"LOGGED FROM: {_ast.FileName} >> {message}";
    }

    /// <summary>
    /// Logs an error message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="pars">Any parameters to log</param>
    public void error(string message, params object?[] pars) => _logger.LogError(GenerateMessage(message), pars);

    /// <summary>
    /// Logs a warning message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="pars">Any parameters to log</param>
    public void warn(string message, params object?[] pars) => _logger.LogWarning(GenerateMessage(message), pars);

    /// <summary>
    /// Logs an informational message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="pars">Any parameters to log</param>
    public void info(string message, params object?[] pars) => _logger.LogInformation(GenerateMessage(message), pars);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="pars">Any parameters to log</param>
    public void debug(string message, params object?[] pars) => _logger.LogDebug(GenerateMessage(message), pars);

    /// <summary>
    /// Logs a trace message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="pars">Any parameters to log</param>
    public void trace(string message, params object?[] pars) => _logger.LogTrace(GenerateMessage(message), pars);
}
