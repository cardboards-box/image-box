namespace ImageBox.Ast;

/// <summary>
///  The dependency injection extensions for the AST parsing
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Add the AST parsing services to the given service collection
    /// </summary>
    /// <param name="services">The service collection to add AST parsing to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAst(this IServiceCollection services)
    {
        return services
            .AddTransient<IAstParserService, AstParserService>();
    }
}
