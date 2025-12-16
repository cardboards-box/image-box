using LoxSmoke.DocXml;

namespace ImageBox.Documentation;

using Models;
using Path = System.IO.Path;

/// <summary>
/// A service for finding type information for documentation generation
/// </summary>
public interface IDocumentReflectionService
{
	/// <summary>
	/// Fetch the class documentation for the given type
	/// </summary>
	/// <param name="settings">The settings to use for doing reflection activities</param>
	/// <param name="type">The type to fetch the data of</param>
	/// <returns>The class information</returns>
	Class Get(IDocumentationSettings settings, Type type);

	/// <summary>
	/// Fetch the enum documentation for the given type
	/// </summary>
	/// <param name="settings">The settings to use for doing reflection activities</param>
	/// <param name="type">The type to fetch the data of</param>
	/// <returns>The enum information</returns>
	Comments Enum(IDocumentationSettings settings, Type type);
}

internal class DocumentReflectionService : IDocumentReflectionService
{
	private string? _nuGetLocation;

	public string? GetNuGetPath()
	{
		if (!string.IsNullOrEmpty(_nuGetLocation)) return _nuGetLocation;

		var location = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		var path = Path.Combine(location, ".nuget", "packages");
		if (Directory.Exists(path)) return _nuGetLocation = path;

		return null;
	}

	public string? FilterNuGetPaths(string[] files)
	{
		//Since NuGet paths start with the version, and append a lang folder
		//with the english version being in the root folder
		//We want to get the latest version, and then get the shortest path
		//otherwise we could get a random language - and everyone speaks English, obviously (/s)
		return files
			.Select(t => new NuGetPath(t, _nuGetLocation!))
			.OrderByDescending(t => t.Version)
			.ThenBy(t => t.NuGetLessPath.Length)
			.FirstOrDefault()?
			.Path;
	}

	public string? GetPath(Assembly assembly)
	{
		var path = Path.ChangeExtension(assembly.Location, ".xml");
		if (File.Exists(path)) return path;

		var fileName = Path.GetFileName(path);
		var nuGet = GetNuGetPath();
		if (nuGet is null) return null;

		var files = Directory.GetFiles(nuGet, fileName, SearchOption.AllDirectories);
		if (files.Length == 0) return null;

		return FilterNuGetPaths(files);
	}

	public static Assembly[] GetAssemblies(IDocumentationSettings settings)
	{
		var it = (settings as DocumentationSettings)!;
		return it._assemblyCache ??= [.. it._assemblies().DistinctBy(t => t.FullName)];
	}

	public DocXmlReader GetReader(IDocumentationSettings settings)
	{
		var it = (settings as DocumentationSettings)!;
		return it._reader ??= new DocXmlReader(GetAssemblies(settings), GetPath);
	}

	public static void FillDefaults(IDocumentationSettings settings)
	{
		var it = (settings as DocumentationSettings)!;
		if (it._typeCache.Count > 0) return;

		//Handle some special types that we don't want to reflection scan
		it._typeCache.Add(typeof(string), new Class(
			"string", typeof(string), [], [],
			new Comments("Represents a sequence of characters.", null, null, null, []), []));
		it._typeCache.Add(typeof(void), new Class(
			"void", typeof(void), [], [],
			new Comments("Represents the absence of a value.", null, null, null, []), []));
		it._typeCache.Add(typeof(object), new Class(
			"object", typeof(object), [], [],
			new Comments("The base class for all types in C#. All types derive from this class.", null, null, null, []), []));
		it._typeCache.Add(typeof(DateTime), new Class(
			"DateTime", typeof(DateTime), [], [],
			new Comments("Represents an instant in time, typically expressed as a date and time of day.", null, null, null, []), []));
		it._typeCache.Add(typeof(TimeSpan), new Class(
			"TimeSpan", typeof(TimeSpan), [], [],
			new Comments("Represents a time interval.", null, null, null, []), []));

		//Fill the comment cache for the default types
		foreach (var (type, item) in it._typeCache)
			it._commentCache[type] = item.Comments;
	}

	public Comments GetComments(IDocumentationSettings settings, Type type)
	{
		var it = (settings as DocumentationSettings)!;
		if (it._commentCache.TryGetValue(type, out var item))
			return item;

		var reader = GetReader(settings);
		var description = reader.GetTypeComments(type);
		var options = !type.IsEnum ? [] : reader.GetEnumComments(type, true)
			.ValueComments
			.Select(t => new TypeOption(t.Name, t.Value, t.Summary))
			.ToArray();
		return it._commentCache[type] = new Comments(
			description.Summary?.ForceNull() ?? string.Empty,
			description.Remarks?.ForceNull(),
			description.Example?.ForceNull(),
			null, options);
	}

	public Comments GetPropertyComments(IDocumentationSettings settings, PropertyInfo property)
	{
		var reader = GetReader(settings);
		var description = reader.GetMemberComments(property);
		return new Comments(
			description.Summary?.ForceNull() ?? string.Empty,
			description.Remarks?.ForceNull(),
			description.Example?.ForceNull(),
			null, []);
	}

	public Comments GetMethodComments(IDocumentationSettings settings, MethodInfo method, out Comments[] paramComments)
	{
		var reader = GetReader(settings);
		var description = reader.GetMethodComments(method);

		paramComments = [..method.GetParameters()
			.Select(t =>
			{
				var comments = description.Parameters
					.Where(p => p.Name == t.Name);
				if (!comments.Any())
					return new Comments("", null, null, null, []);
				var (_, text) = comments.First();
				return new Comments(
					text?.ForceNull() ?? string.Empty,
					null, null, null, []);
			})];

		return new Comments(
			description.Summary?.ForceNull() ?? string.Empty,
			description.Remarks?.ForceNull(),
			description.Example?.ForceNull(),
			description.Returns?.ForceNull(),
			[]);
	}

	public Class Get(IDocumentationSettings settings, Type type)
	{
		var it = (settings as DocumentationSettings)!;
		FillDefaults(settings);
		if (it._typeCache.TryGetValue(type, out var def))
			return def;
		//Get the comments for the type
		var comments = GetComments(settings, type);
		//Primitive types and enums should have they're methods and properties empty
		if (type.IsEnum || type.IsPrimitive)
			return it._typeCache[type] = new Class(type.Name, type, [], [], comments, []);

		var properties = type.GetProperties()
			.Select(t =>
			{
				var comments = GetPropertyComments(settings, t);
				var attrs = it._propertyAttributes(t).ToArray();
				var type = t.PropertyType;
				return new Property(
					t.Name,
					type,
					!t.CanWrite,
					!(type.IsValueType && Nullable.GetUnderlyingType(type) is null),
					comments,
					attrs);
			})
			.ToArray();

		var methods = type.GetMethods()
			.Where(t => !t.IsSpecialName)
			.Select(t =>
			{
				var attrs = it._methodAttributes(t).ToArray();
				var returnType = t.ReturnType;
				var comments = GetMethodComments(settings, t, out var paras);
				var async = typeof(Task).IsAssignableFrom(returnType) ||
							typeof(ValueTask).IsAssignableFrom(returnType);
				var parameters = t.GetParameters()
					.Select((t, i) =>
					{
						var attrs = it._methodParameterAttributes(t).ToArray();
						return new Property(
							t.Name ?? $"param{i}", 
							t.ParameterType, 
							false,
							t.HasDefaultValue,
							paras[i],
							attrs);
					})
					.ToArray();
				return new Method(
					t.Name,
					returnType,
					parameters,
					async,
					comments,
					attrs);
			})
			.ToArray();

		var attrs = it._classAttributes(type).ToArray();
		return it._typeCache[type] = new Class(
			type.Name,
			type,
			properties,
			methods,
			comments,
			attrs);
	}

	public Comments Enum(IDocumentationSettings settings, Type type)
	{
		var it = (settings as DocumentationSettings)!;
		if (it._commentCache.TryGetValue(type, out var item))
			return item;

		var reader = GetReader(settings);
		var description = reader.GetEnumComments(type);
		var options = description.ValueComments
			.Select(t => new TypeOption(t.Name, t.Value, t.Summary))
			.ToArray();

		return it._commentCache[type] = new Comments(
			description.Summary?.ForceNull() ?? string.Empty,
			description.Remarks?.ForceNull(),
			description.Example?.ForceNull(),
			null, options);
	}

	internal record class NuGetPath(
		string Path,
		string NuGetBase)
	{
		private string? _nuGetLessPath;
		private string[]? _segments;

		public string NuGetLessPath
		{
			get
			{
				if (_nuGetLessPath is not null) return _nuGetLessPath;
				if (Path.StartsWith(NuGetBase, StringComparison.OrdinalIgnoreCase))
					return _nuGetLessPath = Path[NuGetBase.Length..];
				return _nuGetLessPath = Path;
			}
		}

		public string[] Segments => _segments ??= NuGetLessPath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);

		public string? Version => Segments.Length > 1 ? Segments[1] : null;
	}
}