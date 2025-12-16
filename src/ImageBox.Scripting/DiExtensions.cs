namespace ImageBox.Scripting;

/// <summary>
/// The extension methods used for adding dependency injection (DI) support to the BoxScript framework.
/// </summary>
public static class DiExtensions
{
	/// <summary>
	/// Adds the script engine services required by the BoxScript framework to the specified service collection.
	/// </summary>
	/// <param name="services">The service collection to add to</param>
	/// <returns>The service collection for fluent method chaining</returns>
	public static IServiceCollection AddScriptingServices(this IServiceCollection services)
	{
		return services
			.AddTransient<IEnumReflectionService, EnumReflectionService>()
			.AddTransient<IScriptEngineService, ScriptEngineService>();
	}
}
