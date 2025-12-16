using System.Reflection;

namespace ImageBox.Documentation;

/// <summary>
/// A fluent builder for documentation settings
/// </summary>
public interface IDocumentationSettings
{
	/// <summary>
	/// The character to use for indenting levels
	/// </summary>
	string IndentCharacter { get; }

	/// <summary>
	/// Use the entry, calling, and executing assembly to find all assemblies to load
	/// </summary>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings AllAssemblies();

	/// <summary>
	/// Specify the assemblies to load the types from
	/// </summary>
	/// <param name="factory">The factory for fetching the assemblies in question</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings Assemblies(Func<IEnumerable<Assembly>> factory);

	/// <summary>
	/// Specify the assemblies to load the types from
	/// </summary>
	/// <param name="assemblies">The assemblies in question</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings Assemblies(IReadOnlyCollection<Assembly> assemblies);

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <typeparam name="T">The attribute type to include</typeparam>
	/// <returns>The current builder for fluent settings</returns>
	/// <remarks>
	/// You can only specify this once, as it overwrites the underlying factory.
	/// If multiple attributes are necessary, use <see cref="ClassAttributes(Type[])"/> 
	/// or <see cref="ClassAttributes(Func{Type, IEnumerable{Attribute}})"/>
	/// </remarks>
	IDocumentationSettings ClassAttribute<T>() where T : Attribute;

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <param name="factory">The factory to fetch the attributes</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings ClassAttributes(Func<Type, IEnumerable<Attribute>> factory);

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <param name="attributeTypes">The attribute types to include</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings ClassAttributes(params Type[] attributeTypes);

	/// <summary>
	/// Sets the <see cref="IndentCharacter"/> to use
	/// </summary>
	/// <param name="indentation">The indentation character to use</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings Indent(string indentation);

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <typeparam name="T">The attribute type to include</typeparam>
	/// <returns>The current builder for fluent settings</returns>
	/// <remarks>
	/// You can only specify this once, as it overwrites the underlying factory.
	/// If multiple attributes are necessary, use <see cref="MethodAttributes(Type[])"/> 
	/// or <see cref="MethodAttributes(Func{MethodInfo, IEnumerable{Attribute}})"/>
	/// </remarks>
	IDocumentationSettings MethodAttribute<T>() where T : Attribute;

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="factory">The factory to fetch the attributes</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings MethodAttributes(Func<MethodInfo, IEnumerable<Attribute>> factory);

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="attributeTypes">The attribute types to include</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings MethodAttributes(params Type[] attributeTypes);

	/// <summary>
	/// All properties scanned must match the given filter
	/// </summary>
	/// <param name="filter">The filter to match</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings MethodFilter(Func<MethodInfo, bool> filter);

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <typeparam name="T">The attribute type to include</typeparam>
	/// <returns>The current builder for fluent settings</returns>
	/// <remarks>
	/// You can only specify this once, as it overwrites the underlying factory.
	/// If multiple attributes are necessary, use <see cref="PropertyAttributes(Type[])"/> 
	/// or <see cref="PropertyAttributes(Func{PropertyInfo, IEnumerable{Attribute}})"/>
	/// </remarks>
	IDocumentationSettings ParameterAttribute<T>() where T : Attribute;

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="factory">The factory to fetch the attributes</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings ParameterAttributes(Func<ParameterInfo, IEnumerable<Attribute>> factory);

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="attributeTypes">The attribute types to include</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings ParameterAttributes(params Type[] attributeTypes);

	/// <summary>
	/// Include the given attributes when resolving classes
	/// </summary>
	/// <typeparam name="T">The attribute type to include</typeparam>
	/// <returns>The current builder for fluent settings</returns>
	/// <remarks>
	/// You can only specify this once, as it overwrites the underlying factory.
	/// If multiple attributes are necessary, use <see cref="ParameterAttributes(Type[])"/> 
	/// or <see cref="ParameterAttributes(Func{ParameterInfo, IEnumerable{Attribute}})"/>
	/// </remarks>
	IDocumentationSettings PropertyAttribute<T>() where T : Attribute;

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="factory">The factory to fetch the attributes</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings PropertyAttributes(Func<PropertyInfo, IEnumerable<Attribute>> factory);

	/// <summary>
	/// Include the given attributes when resolving properties
	/// </summary>
	/// <param name="attributeTypes">The attribute types to include</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings PropertyAttributes(params Type[] attributeTypes);

	/// <summary>
	/// All properties scanned must match the given filter
	/// </summary>
	/// <param name="filter">The filter to match</param>
	/// <returns>The current builder for fluent settings</returns>
	IDocumentationSettings PropertyFilter(Func<PropertyInfo, bool> filter);
}