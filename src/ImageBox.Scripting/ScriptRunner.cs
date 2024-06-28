using Jint;
using Jint.Native;
using Jint.Runtime.Modules;
using Module = Esprima.Ast.Module;

namespace ImageBox.Scripting;

/// <summary>
/// Represents a thread safe instance of a JavaScript executor
/// </summary>
/// <param name="timeoutSec">The max time a script can be running for in seconds</param>
/// <param name="recursion">The max number of recursive calls that can be done</param>
/// <param name="memoryLimitMb">The max amount of memory for each script in megabytes</param>
/// <remarks>These should be cached and are safe to run multiple times (even at the same time!)</remarks>
public class ScriptRunner(
    double timeoutSec = 10,
    int recursion = 900,
    double memoryLimitMb = 4)
{
    /// <summary>
    /// Random number generator used for generating the main module name if non is set.
    /// Used primarily in <see cref="GenerateMainName"/>
    /// </summary>
    private static readonly Random _rnd = new();

    /// <summary>
    /// Store all of the prepared modules that have been loaded from files or from strings
    /// </summary>
    private readonly Dictionary<string, Prepared<Module>> _modules = [];

    /// <summary>
    /// Store all of the module builders for customized modules (prefer prepared modules if possible)
    /// </summary>
    private readonly Dictionary<string, Action<ModuleBuilder>> _moduleBuilders = [];

    /// <summary>
    /// Prepare the default main module
    /// </summary>
    private Prepared<Module>? _main;

    /// <summary>
    /// The optional name of the main module (will be generated if not set - <see cref="GenerateMainName"/>)
    /// </summary>
    private string? _mainName;

    /// <summary>
    /// Checks to see if a module of the given name exists
    /// </summary>
    /// <param name="name">The name of the module</param>
    /// <returns>Whether or not the module exists</returns>
    public bool ModuleExists(string name) => _modules.ContainsKey(name) || _moduleBuilders.ContainsKey(name);

    /// <summary>
    /// Add a module to the script engine
    /// </summary>
    /// <param name="name">The name of the module (used in the import statement)</param>
    /// <param name="code">The code of the module</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when a module with the same name already exists</exception>
    public ScriptRunner AddModule(string name, string code)
    {
        return AddModule(name, Prepare(code));
    }

    /// <summary>
    /// Add a module to the script engine
    /// </summary>
    /// <param name="name">The name of the module (used in the import statement)</param>
    /// <param name="module">The code of the module</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when a module with the same name already exists</exception>
    public ScriptRunner AddModule(string name, Prepared<Module> module)
    {
        if (ModuleExists(name))
            throw new ArgumentException($"Module with name {name} already exists");

        _modules.Add(name, module);
        return this;
    }

    /// <summary>
    /// Loads a module from a file
    /// </summary>
    /// <param name="filePath">The path to the module script</param>
    /// <param name="name">The name of the module (used in the import statement)</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <remarks><paramref name="name"/> defaults to the name of the file from the <paramref name="filePath"/> with the extension</remarks>
    /// <exception cref="ArgumentException">Thrown when a module with the same name already exists</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file doesn't exist</exception>
    public ScriptRunner LoadModule(string filePath, string? name = null)
    {
        var (moduleName, code) = PrepareFile(filePath, name);
        return AddModule(moduleName, code);
    }

    /// <summary>
    /// Loads a module from a builder.
    /// </summary>
    /// <param name="name">The name of the module</param>
    /// <param name="builder">The builder method for the module</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>Prefer <see cref="AddModule(string, string)"/> or <see cref="LoadModule(string, string?)"/> for better performance</remarks>
    public ScriptRunner AddModule(string name, Action<ModuleBuilder> builder)
    {
        if (ModuleExists(name))
            throw new ArgumentException($"Module with name {name} already exists");

        _moduleBuilders.Add(name, builder);
        return this;
    }

    /// <summary>
    /// Adds the main script to the engine.
    /// </summary>
    /// <param name="code">The main script</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <remarks>This should contain a `main(args[])` entry point</remarks>
    public ScriptRunner SetScript(string code)
    {
        return SetScript(Prepare(code));
    }

    /// <summary>
    /// Adds the main script to the engine.
    /// </summary>
    /// <param name="script">The main script</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    /// <remarks>This should contain a `main(args[])` entry point</remarks>
    public ScriptRunner SetScript(Prepared<Module> script)
    {
        _main = script;
        return this;
    }

    /// <summary>
    /// Sets the name of the main module
    /// </summary>
    /// <param name="name">The name of the module</param>
    /// <returns>The current instance of the script runner for method chaining</returns>
    public ScriptRunner SetMainModuleName(string name)
    {
        _mainName = name;
        return this;
    }

    /// <summary>
    /// Executes the script
    /// </summary>
    /// <param name="parameters">The parameters to pass into the `main(args[])` function</param>
    /// <param name="token">The cancellation token for the execution</param>
    /// <returns>The return result from the `main(args[])` function</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetScript(string)"/> hasn't been called</exception>
    /// <exception cref="ArgumentException">Thrown if the main module doesn't have a `main(args[])` function</exception>
    public Task<JsValue?> Execute(object?[] parameters, CancellationToken token)
    {
        return Task.Run(() => ExecuteAsync(parameters, token), token);
    }

    /// <summary>
    /// Executes the script
    /// </summary>
    /// <param name="parameters">The parameters to pass into the `main(args[])` function</param>
    /// <returns>The return result from the `main(args[])` function</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetScript(string)"/> hasn't been called</exception>
    /// <exception cref="ArgumentException">Thrown if the main module doesn't have a `main(args[])` function</exception>
    public Task<JsValue?> Execute(params object?[] parameters)
    {
        return Execute(parameters, CancellationToken.None);
    }

    /// <summary>
    /// Executes the script
    /// </summary>
    /// <param name="token">The cancellation token source for the execution (can be used to cancel the script execution)</param>
    /// <param name="parameters">The parameters to pass into the `main(args[])` function</param>
    /// <returns>The return result from the `main(args[])` function</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetScript(string)"/> hasn't been called</exception>
    /// <exception cref="ArgumentException">Thrown if the main module doesn't have a `main(args[])` function</exception>
    public Task<JsValue?> Execute(out CancellationTokenSource token, params object?[] parameters)
    {
        token = new CancellationTokenSource();
        return Execute(parameters, token.Token);
    }

    /// <summary>
    /// Generates a random name for the main module
    /// </summary>
    /// <returns>The random name that doesn't conflict with other modules</returns>
    private string GenerateMainName()
    {
        static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return "main" + new string(Enumerable.Repeat(chars, 10).Select(s => s[_rnd.Next(s.Length)]).ToArray());
        }

        var name = RandomString();
        while (_modules.ContainsKey(name))
            name = RandomString();

        return name;
    }

    /// <summary>
    /// Converts the result of <see cref="Run(object?[], CancellationToken)"/> to a task for threading
    /// </summary>
    /// <param name="parameters">The parameters to pass into the `main(args[])` function</param>
    /// <param name="token">The cancellation token for the execution</param>
    /// <returns>The return result from the `main(args[])` function</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetScript(string)"/> hasn't been called</exception>
    /// <exception cref="ArgumentException">Thrown if the main module doesn't have a `main(args[])` function</exception>
    private Task<JsValue?> ExecuteAsync(object?[] parameters, CancellationToken? token)
    {
        var cancel = token ?? CancellationToken.None;
        return Task.FromResult(Run(parameters, cancel));
    }

    /// <summary>
    /// Executes the script in a thread-safe manner
    /// </summary>
    /// <param name="parameters">The parameters to pass into the `main(args[])` function</param>
    /// <param name="token">The cancellation token for the execution</param>
    /// <returns>The return result from the `main(args[])` function</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="SetScript(string)"/> hasn't been called</exception>
    /// <exception cref="ArgumentException">Thrown if the main module doesn't have a `main(args[])` function</exception>
    private JsValue? Run(object?[] parameters, CancellationToken token)
    {
        if (_main is null)
            throw new InvalidOperationException("Main script not set");

        //Create the engine instance
        using var engine = new Engine(c => c
            .LimitRecursion(recursion)
            .TimeoutInterval(TimeSpan.FromSeconds(timeoutSec))
            .CancellationToken(token)
            .LimitMemory((long)(memoryLimitMb * 1024 * 1024))
        );

        //Include all of our modules
        foreach (var (name, value) in _modules)
            engine.Modules.Add(name, x => x.AddModule(value));
        foreach (var (name, builder) in _moduleBuilders)
            engine.Modules.Add(name, builder);

        //Get the name of the main module
        var mainName = _mainName ?? GenerateMainName();
        //Include the main module
        engine.Modules.Add(mainName, x => x.AddModule(_main.Value));
        //Import the main module and get it's context
        var ns = engine.Modules.Import(mainName);
        //Get the main function from the main module
        var method = ns.Get("main") ?? throw new ArgumentException("`main` function not found");
        //Invoke the main function with the given parameters
        var result = engine.Invoke(method, parameters);
        //Process any tasks that are pending
        engine.Advanced.ProcessTasks();
        //Massage the result to either be null or the result of the return value
        return result == JsValue.Undefined ? null : result;
    }

    /// <summary>
    /// Prepares the script for module execution
    /// </summary>
    /// <param name="code">The code to prepare</param>
    /// <returns>The prepared module</returns>
    public static Prepared<Module> Prepare(string code)
    {
        return Engine.PrepareModule(code);
    }

    /// <summary>
    /// Prepares the script for module execution from a file
    /// </summary>
    /// <param name="path">The path to load the code from</param>
    /// <param name="name">The name of the module</param>
    /// <returns>The prepared module</returns>
    public static (string name, Prepared<Module> module) PrepareFile(string path, string? name = null)
    {
        var task = PrepareFileAsync(path, name);
        task.Wait();
        return task.Result;
    }

    /// <summary>
    /// Prepares the script for module execution from a file
    /// </summary>
    /// <param name="path">The path to load the code from</param>
    /// <param name="name">The name of the module</param>
    /// <returns>The prepared module</returns>
    public static async Task<(string name, Prepared<Module> module)> PrepareFileAsync(string path, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = Path.GetFileNameWithoutExtension(path);

        if (!File.Exists(path))
            throw new FileNotFoundException("Module file not found", Path.GetFullPath(path));

        var code = await File.ReadAllTextAsync(path);
        return (name, Prepare(code));
    }
}
