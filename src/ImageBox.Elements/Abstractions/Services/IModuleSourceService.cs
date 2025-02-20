using Jint.Runtime.Modules;

namespace ImageBox.Elements;

/// <summary>
/// Represents a service that provides modules to the script files
/// </summary>
public interface IModuleSourceService
{
    /// <summary>
    /// The name of the module to use
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers the module with the given builder
    /// </summary>
    Task<Action<ModuleBuilder>> Register(LoadedAst ast);
}
