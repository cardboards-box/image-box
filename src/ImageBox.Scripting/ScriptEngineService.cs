namespace ImageBox.Scripting;

using Serilog;
using static Constants;

/// <summary>
/// The service responsible for executing box-scripts
/// </summary>
public interface IScriptEngineService
{
	/// <summary>
	/// Creates a new instance of the script engine settings
	/// </summary>
	/// <returns>The settings instance</returns>
	IScriptEngineSettings SettingsInstance();

	/// <summary>
	/// Generate a service collection from the given settings
	/// </summary>
	/// <param name="settings">The settings</param>
	/// <returns>The service collection</returns>
	IServiceCollection GenerateServiceCollection(IScriptEngineSettings settings);

	/// <summary>
	/// Describes all of the enums that are registered in the script engine settings
	/// </summary>
	/// <param name="settings">The engine settings</param>
	/// <returns>The enum descriptions</returns>
	IEnumerable<EnumDescription> Enums(IScriptEngineSettings settings);

	/// <summary>
	/// Executes a script and returns whether it was successful or not
	/// </summary>
	/// <param name="script">The script to run</param>
	/// <param name="bob">The configuration builder</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> Execute(Stream script, Action<IScriptEngineSettings> bob);

	/// <summary>
	/// Executes a script and returns whether it was successful or not
	/// </summary>
	/// <param name="script">The script to run</param>
	/// <param name="settings">The settings to use for the execution</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> Execute(Stream script, IScriptEngineSettings settings);

	/// <summary>
	/// Executes a script from a file path and returns whether it was successful or not
	/// </summary>
	/// <param name="path">The path to the script</param>
	/// <param name="settings">The settings to use for the execution</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> ExecuteFromPath(string path, IScriptEngineSettings settings);

	/// <summary>
	/// Executes a script from a file path and returns whether it was successful or not
	/// </summary>
	/// <param name="path">The path to the script</param>
	/// <param name="bob">The configuration builder</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> ExecuteFromPath(string path, Action<IScriptEngineSettings> bob);

	/// <summary>
	/// Executes a script and returns whether it was successful or not
	/// </summary>
	/// <param name="script">The script to run</param>
	/// <param name="bob">The configuration builder</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> Execute(string script, Action<IScriptEngineSettings> bob);

	/// <summary>
	/// Executes a script and returns whether it was successful or not
	/// </summary>
	/// <param name="script">The script to run</param>
	/// <param name="settings">The settings to use for the execution</param>
	/// <returns>Whether or not the script was successful</returns>
	Task<JsValue?> Execute(string script, IScriptEngineSettings settings);
}

internal class ScriptEngineService(
	ILogger<ScriptEngineService> _logger,
	IEnumReflectionService _enums) : IScriptEngineService
{
	/// <summary>
	/// Stores the name of the <see cref="IScriptModule"/> fetched via reflection for caching
	/// </summary>
	private static readonly ConcurrentDictionary<Type, string> _moduleNameCache = [];

	public IScriptEngineSettings SettingsInstance()
	{
		return new ScriptEngineSettings();
	}

	#region Stream based loading
	public Task<JsValue?> Execute(Stream script, Action<IScriptEngineSettings> bob)
	{
		var settings = new ScriptEngineSettings();
		bob(settings);
		return Execute(script, settings);
	}

	public async Task<JsValue?> Execute(Stream script, IScriptEngineSettings settings)
	{
		using var sr = new StreamReader(script);
		var content = await sr.ReadToEndAsync();
		return await Execute(content, settings);
	}
	#endregion

	#region Path based execution
	public Task<JsValue?> ExecuteFromPath(string path, Action<IScriptEngineSettings> bob)
	{
		var settings = new ScriptEngineSettings();
		bob(settings);
		return ExecuteFromPath(path, settings);
	}

	public async Task<JsValue?> ExecuteFromPath(string path, IScriptEngineSettings settings)
	{
		if (!File.Exists(path))
		{
			_logger.LogWarning("Script file not found at path: {Path}", path);
			throw new FileNotFoundException("Script file not found", path);
		}

		using var sr = File.OpenText(path);
		var content = await sr.ReadToEndAsync();
		return await Execute(content, settings);
	}
	#endregion

	#region String based execution
	public Task<JsValue?> Execute(string script, Action<IScriptEngineSettings> bob)
	{
		var settings = new ScriptEngineSettings();
		bob(settings);
		return Execute(script, settings);
	}

	public async Task<JsValue?> Execute(string script, IScriptEngineSettings settings)
	{
		if (!settings.RunInBackground)
			return Run(script, settings);

		return await Task.Run(() => Run(script, settings), settings.CancelToken);
	}
	#endregion

	public IServiceCollection GenerateServiceCollection(IScriptEngineSettings settings)
	{
		var services = new ServiceCollection();
		//Ensure the service actions are registered
		foreach (var action in settings.Services)
			action(services);

		//Ensure the configuration actions are registered
		var cb = new ConfigurationBuilder()
			.AddCommandLine(Environment.GetCommandLineArgs())
			.AddEnvironmentVariables();

		foreach (var action in settings.Configs)
			action(cb);

		return services
			.AddSingleton(settings)
			.AddSingleton<IConfiguration>(cb.Build())
			.AddSerilog(c =>
			{
				c.WriteTo.Console();
				//Ensure all of the logger actions are registered
				foreach (var action in settings.Loggers)
					action(c);
			});
	}

	public JsValue? Run(string script, IScriptEngineSettings settings)
	{
		//Create an instance of the script engine
		using var engine = CreateEngine(settings);
		var services = GenerateServiceCollection(settings);
		//Add the engine to the service collection
		services.AddSingleton(engine);
		//Build the service provider
		var provider = services.BuildServiceProvider();
		//Add the service provider to the service collection
		services.AddSingleton<IServiceProvider>(provider);
		//Add all of the modules to the engine
		engine.Modules.Add(MODULE_NAME, t =>
		{
			foreach (var module in provider.GetServices<IScriptModule>())
			{
				var name = ResolveModuleName(module);
				var proxy = new ScriptProxy(module);
				t.ExportObject(name, proxy);
			}
		});
		//Prepare the main method to be wrapped in a module
		var mainMethodName = MAIN_METHOD_PREFIX + 10.RandomString();
		var mainModule = PrepareMainMethod(script, mainMethodName, settings);
		//Add the main module to the engine with a random name to avoid collisions
		var mainModuleName = MAIN_MODULE_PREFIX + 10.RandomString();
		engine.Modules.Add(mainModuleName, mainModule);
		//Get the main module instance from the modules
		var mainInstance = engine.Modules.Import(mainModuleName);
		if (mainInstance is null)
		{
			_logger.LogError("Could not find main module: {ModuleName}", mainModuleName);
			throw new InvalidOperationException($"Could not find main module: {mainModuleName}");
		}
		//Get the main method from the main module
		var method = mainInstance.Get(mainMethodName);
		if (method is null)
		{
			_logger.LogError("Could not find main method: {MethodName} in module: {ModuleName}", mainMethodName, mainModuleName);
			throw new InvalidOperationException($"Could not find main method: {mainMethodName} in module: {mainModuleName}");
		}
		//Invoke the main method
		var result = engine.Invoke(method);
		//Ensure the engine is done processing background tasks
		engine.Advanced.ProcessTasks();
		//Unwrap the result of the promises
		result = result.UnwrapIfPromise();
		//Return the result of the script execution
		return result == JsValue.Undefined || result == JsValue.Null
			? null : result;
	}

	public string PrepareMainMethod(string script, string name, IScriptEngineSettings settings)
	{
		var sections = ParseScriptSections(script).ToArray();
		var imports = string.Join("\n", sections.Take(sections.Length - 1));
		var body = string.Join("\n", sections.Last()
			.Split('\n')
			.Select(t => $"\t{t}"));

		var enums = GenerateEnums(settings);

		return $"{imports}\n{enums}\nexport async function {name}() {{\n{body}\n}}";
	}

	/// <summary>
	/// Parses the given script into sections. 
	/// </summary>
	/// <param name="script">The script to parse</param>
	/// <returns>The sections of the script</returns>
	/// <remarks>Each section is an import statement, and the last section is the body of the script</remarks>
	public IEnumerable<string> ParseScriptSections(string script)
	{
		(string start, string end)[] sections =
		[
			("import", ";"),
			("//", "\n"),
			("/*", "*/")
		];

		//purge windows new-line and prefer unix
		script = script.Replace("\r\n", "\n");

		int current = 0;
		while (current < script.Length)
		{
			//Find the next starting index section
			var next = sections
				.Select(t => (t.start, t.end, index: script.IndexOf(t.start, current)))
				.Where(t => t.index >= 0);
			//If there are none - we've hit the end of the script
			if (!next.Any())
			{
				//If the current index is not at the end of the script, return the rest of the script as the body
				if (current < script.Length)
					yield return script[current..].Trim();
				yield break;
			}
			//Get the lowest index of the next section
			var (start, end, index) = next.MinBy(t => t.index);

			//If the content between the current index and the next index is not whitespace, it's the body of the script
			if (!string.IsNullOrWhiteSpace(script[current..index]))
			{
				//Return the rest of the script as the body
				yield return script[current..].Trim();
				//Stop processing
				yield break;
			}

			//Find the end of the section
			var endIndex = script.IndexOf(end, index + start.Length);
			if (endIndex < 0)
			{
				_logger.LogWarning("Could not find end of the section ({end}) for {start}({current} + {index}) from script: {script}",
					end, start, current, index, script);
				throw new InvalidOperationException(
					$"Could not find end of the section ({end}) for {start}({current} + {index}) from script: {script}");
			}

			//Return the section
			yield return script[index..(endIndex + end.Length)].Trim();
			//Move the current index to the end of the section
			current = endIndex + end.Length;
		}
	}

	public IEnumerable<EnumDescription> Enums(IScriptEngineSettings settings)
	{
		return settings.Enums
			.Select(_enums.Describe);
	}

	public string GenerateEnums(IScriptEngineSettings settings)
	{
		var enums = Enums(settings);
		var bob = new StringBuilder();
		foreach (var description in enums)
			AppendEnumClass(bob, description);
		return bob.ToString();
	}

	public static void AppendEnumClass(StringBuilder bob, EnumDescription description)
	{
		bob.AppendLine($"class {description.Name} {{");
		foreach (var value in description.Values)
			bob.AppendLine($"\tstatic get {value.Name}() {{ return {value.Value}; }}");
		bob.AppendLine("}");
		bob.AppendLine();
	}

	public static string ResolveModuleName(IScriptModule module)
	{
		var type = module.GetType();

		if (!_moduleNameCache.TryGetValue(type, out var name) ||
			string.IsNullOrEmpty(name))
		{
			var attr = type.GetCustomAttribute<ModuleAttribute>();
			name = attr?.Name ?? type.Name;
			_moduleNameCache[type] = name;
		}

		return name;
	}

	public static Engine CreateEngine(IScriptEngineSettings settings)
	{
		return new Engine(cfg =>
		{
			if (settings.ExecutionTimeout > TimeSpan.Zero)
				cfg.TimeoutInterval(settings.ExecutionTimeout);

			if (settings.MemoryLimitMb > 0)
				cfg.LimitMemory((long)(settings.MemoryLimitMb * 1024 * 1024)); // Convert MB to bytes

			if (settings.RecursionLimit > 0)
				cfg.LimitRecursion(settings.RecursionLimit);

			cfg.CancellationToken(settings.CancelToken);
			cfg.EnableModules(AppDomain.CurrentDomain.BaseDirectory, true);
		});
	}
}
