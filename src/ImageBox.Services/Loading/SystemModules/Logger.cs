namespace ImageBox.Services.Loading.SystemModules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
internal class Logger(ILogger _logger, LoadedAst _ast)
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    public string GenerateMessage(string message)
    {
        return $"LOGGED FROM: {_ast.FileName} >> {message}";
    }

    public void error(string message, params object?[] pars) => _logger.LogError(GenerateMessage(message), pars);

    public void warn(string message, params object?[] pars) => _logger.LogWarning(GenerateMessage(message), pars);

    public void info(string message, params object?[] pars) => _logger.LogInformation(GenerateMessage(message), pars);

    public void debug(string message, params object?[] pars) => _logger.LogDebug(GenerateMessage(message), pars);

    public void trace(string message, params object?[] pars) => _logger.LogTrace(GenerateMessage(message), pars);
}
