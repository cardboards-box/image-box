namespace ImageBox.Scripting;

/// <summary>
/// The service that allows for reflection of enums.
/// </summary>
public interface IEnumReflectionService
{
	/// <summary>
	/// Describes an enum type
	/// </summary>
	/// <param name="type">The type of enum</param>
	/// <returns>The description of the enum</returns>
	EnumDescription Describe(Type type);

	/// <summary>
	/// Describes an enum type
	/// </summary>
	/// <typeparam name="T">The type of enum</typeparam>
	/// <returns>The description of the enum</returns>
	EnumDescription Describe<T>() where T : Enum;
}

internal class EnumReflectionService : IEnumReflectionService
{
	private static readonly ConcurrentDictionary<Type, EnumDescription> _cache = [];

	public EnumDescription Describe(Type type)
	{
		if (_cache.TryGetValue(type, out var desc))
			return desc;

		if (!type.IsEnum)
			throw new ArgumentException($"Type {type.FullName} is not an enum.", nameof(type));

		var name = type.Name;
		var values = Enum.GetValues(type)
			.Cast<Enum>()
			.Select(e => new EnumValue(e.ToString(), Convert.ToInt64(e), e.GetType()))
			.ToArray();
		return _cache[type] = new EnumDescription(name, type, values);
	}

	public EnumDescription Describe<T>() where T : Enum
	{
		return Describe(typeof(T));
	}
}