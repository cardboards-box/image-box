using Serilog;

namespace ImageBox.Scripting;

/// <summary>
/// A fluent chaining settings builder for script engine
/// </summary>
public interface IScriptEngineSettings
{
	/// <summary>
	/// The maximum duration a single script can run for
	/// </summary>
	/// <remarks>Defaults to 300 seconds</remarks>
	TimeSpan ExecutionTimeout { get; }

	/// <summary>
	/// The maximum number of times a method within the script can call itself recursively before it is aborted
	/// </summary>
	/// <remarks>Defaults to 999</remarks>
	int RecursionLimit { get; }

	/// <summary>
	/// The maximum amount of memory a script can use before it is aborted
	/// </summary>
	/// <remarks>Defaults to 1/2GB</remarks>
	double MemoryLimitMb { get; }

	/// <summary>
	/// The cancellation token that can be used to cancel the script execution
	/// </summary>
	CancellationToken CancelToken { get; }

	/// <summary>
	/// Whether or not to run the script on a separate thread than the calling one
	/// </summary>
	/// <remarks>Defaults to false</remarks>
	bool RunInBackground { get; }

	/// <summary>
	/// The collection of service configuration actions to be applied to the service collection
	/// </summary>
	IEnumerable<Action<IServiceCollection>> Services { get; }

	/// <summary>
	/// The collection of configuration builder actions to be applied to the configuration builder
	/// </summary>
	IEnumerable<Action<IConfigurationBuilder>> Configs { get; }

	/// <summary>
	/// The collection of logger configuration actions to be applied to the logger configuration
	/// </summary>
	IEnumerable<Action<LoggerConfiguration>> Loggers { get; }

	/// <summary>
	/// All of the enums that should be generated into the script
	/// </summary>
	IEnumerable<Type> Enums { get; }

	/// <summary>
	/// Adds an enum type to the script engine settings, allowing it to be used in scripts
	/// </summary>
	/// <typeparam name="T">The enum type</typeparam>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddEnum<T>() where T : Enum;

	/// <summary>
	/// Sets the maximum duration a single script can run for
	/// </summary>
	/// <param name="timeout">The duration the script can run for</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	/// <remarks>Set to <see cref="TimeSpan.Zero"/> to disable timeouts</remarks>
	IScriptEngineSettings SetExecutionTimeout(TimeSpan timeout);

	/// <summary>
	/// Sets the maximum duration a single script can run for in seconds
	/// </summary>
	/// <param name="seconds">The number of seconds the script can run for</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	/// <remarks>Set to 0 to disable timeouts</remarks>
	IScriptEngineSettings SetExecutionTimeout(double seconds) => SetExecutionTimeout(TimeSpan.FromSeconds(seconds));

	/// <summary>
	/// Sets the maximum number of times a method within the script can call itself recursively before it is aborted
	/// </summary>
	/// <param name="limit">The maximum number of times a method within the script can call itself recursively before it is aborted</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	/// <remarks>Set to 0 to disable recursion limits</remarks>
	IScriptEngineSettings SetRecursionLimit(int limit);

	/// <summary>
	/// Sets the maximum amount of memory a script can use before it is aborted
	/// </summary>
	/// <param name="mb">The maximum amount of memory a script can use before it is aborted</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	/// <remarks>Set to 0 to disable memory limits</remarks>
	IScriptEngineSettings SetMemoryLimit(double mb);

	/// <summary>
	/// Sets the cancellation token that can be used to cancel the script execution
	/// </summary>
	/// <param name="token">The cancellation token to use</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings SetCancelToken(CancellationToken token);

	/// <summary>
	/// Sets whether or not to run the script on a separate thread than the calling one
	/// </summary>
	/// <param name="background">Whether or not to run the script on a separate thread than the calling one</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings SetRunInBackground(bool background);

	/// <summary>
	/// Adds a module to the script engine
	/// </summary>
	/// <typeparam name="T">The type of the module</typeparam>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddModule<T>() where T : class, IScriptModule;

	/// <summary>
	/// Adds a module instance to the script engine
	/// </summary>
	/// <typeparam name="T">The type of the module</typeparam>
	/// <param name="instance">The instance of the module to add</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddModule<T>(T instance) where T : class, IScriptModule;

	/// <summary>
	/// Allows access to the service collection for adding other services
	/// </summary>
	/// <param name="services">The service collection configuration action</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddServices(Action<IServiceCollection> services);

	/// <summary>
	/// Adds a configuration action to the script engine settings
	/// </summary>
	/// <param name="bob">The configuration builder action</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddConfig(Action<IConfigurationBuilder> bob);

	/// <summary>
	/// Adds a configuration file to the script engine settings
	/// </summary>
	/// <param name="path">The file path</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	/// <remarks>Supports JSON, INI, and XML files</remarks>
	IScriptEngineSettings AddConfigFile(string path);

	/// <summary>
	/// Adds a logger configuration action to the script engine settings
	/// </summary>
	/// <param name="logger">The logger configuration builder</param>
	/// <returns>The settings engine for fluent method chaining</returns>
	IScriptEngineSettings AddLogger(Action<LoggerConfiguration> logger);
}

internal class ScriptEngineSettings : IScriptEngineSettings
{
	private readonly List<Action<IServiceCollection>> _services = [];
	private readonly List<Action<IConfigurationBuilder>> _configs = [];
	private readonly List<Action<LoggerConfiguration>> _loggers = [];
	private readonly HashSet<Type> _enums = [];

	public TimeSpan ExecutionTimeout { get; set; } = TimeSpan.FromSeconds(300);

	public int RecursionLimit { get; set; } = 999;

	public double MemoryLimitMb { get; set; } = 500.0;

	public CancellationToken CancelToken { get; set; } = CancellationToken.None;

	public bool RunInBackground { get; set; } = false;

	public IEnumerable<Action<IServiceCollection>> Services => _services;

	public IEnumerable<Action<IConfigurationBuilder>> Configs => _configs;

	public IEnumerable<Action<LoggerConfiguration>> Loggers => _loggers;

	public IEnumerable<Type> Enums => _enums;

	public IScriptEngineSettings AddEnum<T>() where T : Enum
	{
		_enums.Add(typeof(T));
		return this;
	}

	public IScriptEngineSettings AddLogger(Action<LoggerConfiguration> logger)
	{
		_loggers.Add(logger);
		return this;
	}

	public IScriptEngineSettings AddConfig(Action<IConfigurationBuilder> bob)
	{
		_configs.Add(bob);
		return this;
	}

	public IScriptEngineSettings AddConfigFile(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new ArgumentException("Configuration file path cannot be null or empty.", nameof(path));

		if (!File.Exists(path))
			throw new FileNotFoundException("Configuration file not found.", path);

		return AddConfig(c => c
			.AddFile(path, optional: true, reloadOnChange: true));
	}

	public IScriptEngineSettings AddModule<T>() where T : class, IScriptModule
	{
		return AddServices(t => t.AddTransient<IScriptModule, T>());
	}

	public IScriptEngineSettings AddModule<T>(T instance) where T : class, IScriptModule
	{
		return AddServices(t => t.AddSingleton<IScriptModule>(instance));
	}

	public IScriptEngineSettings AddServices(Action<IServiceCollection> services)
	{
		_services.Add(services);
		return this;
	}

	public IScriptEngineSettings SetRunInBackground(bool background)
	{
		RunInBackground = background;
		return this;
	}

	public IScriptEngineSettings SetCancelToken(CancellationToken token)
	{
		CancelToken = token;
		return this;
	}

	public IScriptEngineSettings SetExecutionTimeout(TimeSpan timeout)
	{
		if (timeout < TimeSpan.Zero) timeout = TimeSpan.Zero;
		ExecutionTimeout = timeout;
		return this;
	}

	public IScriptEngineSettings SetMemoryLimit(double mb)
	{
		if (mb < 0) mb = 0;
		MemoryLimitMb = mb;
		return this;
	}

	public IScriptEngineSettings SetRecursionLimit(int limit)
	{
		if (limit < 0) limit = 0;
		RecursionLimit = limit;
		return this;
	}
}
