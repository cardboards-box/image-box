using Jint;
using Jint.Native;

namespace ImageBox.Core;

/// <summary>
/// A collection of extensions
/// </summary>
public static class Extensions
{
	/// <summary>
	/// The default random characters to use for <see cref="RandomString(int, string?, Random?)"/>
	/// </summary>
	public const string DEFAULT_RANDOM_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	/// <summary>
	/// The random instance to use for random methods
	/// </summary>
	public static Random RandomInstance { get; } = new Random();

	/// <summary>
	/// Generates a random string of the specified length
	/// </summary>
	/// <param name="length">The length of the string</param>
	/// <param name="characters">The available characters to use for generation</param>
	/// <param name="rnd">The random instance to use for random-ness</param>
	/// <returns>The random string of the given length</returns>
	public static string RandomString(this int length, string? characters = null, Random? rnd = null)
	{
		characters ??= DEFAULT_RANDOM_CHARACTERS;
		rnd ??= RandomInstance;

		if (string.IsNullOrWhiteSpace(characters))
			throw new ArgumentException("Characters cannot be null or whitespace", nameof(characters));

		return new string(Enumerable
			.Repeat('a', length)
			.Select(s => characters[rnd.Next(characters.Length)])
			.ToArray());
	}

	/// <summary>
	/// Converts a method into a delegate for the specified instance
	/// </summary>
	/// <param name="method">The method to convert</param>
	/// <param name="instance">The object the method is attached to</param>
	/// <returns>The delegate created from the method</returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if there are more than 16 parameters for the method</exception>
	public static Delegate ToDelegate(this MethodInfo method, object instance)
	{
		static Type MakeFuncParameter(int parameters)
		{
			return parameters switch
			{
				00 => typeof(Func<>),
				01 => typeof(Func<,>),
				02 => typeof(Func<,,>),
				03 => typeof(Func<,,,>),
				04 => typeof(Func<,,,,>),
				05 => typeof(Func<,,,,,>),
				06 => typeof(Func<,,,,,,>),
				07 => typeof(Func<,,,,,,,>),
				08 => typeof(Func<,,,,,,,,>),
				09 => typeof(Func<,,,,,,,,,>),
				10 => typeof(Func<,,,,,,,,,,>),
				11 => typeof(Func<,,,,,,,,,,,>),
				12 => typeof(Func<,,,,,,,,,,,,>),
				13 => typeof(Func<,,,,,,,,,,,,,>),
				14 => typeof(Func<,,,,,,,,,,,,,,>),
				15 => typeof(Func<,,,,,,,,,,,,,,,>),
				16 => typeof(Func<,,,,,,,,,,,,,,,,>),
				_ => throw new ArgumentOutOfRangeException(nameof(parameters), "Too many parameters for Func")
			};
		}

		static Type MakeActionParameter(int parameters)
		{
			return parameters switch
			{
				00 => typeof(Action),
				01 => typeof(Action<>),
				02 => typeof(Action<,>),
				03 => typeof(Action<,,>),
				04 => typeof(Action<,,,>),
				05 => typeof(Action<,,,,>),
				06 => typeof(Action<,,,,,>),
				07 => typeof(Action<,,,,,,>),
				08 => typeof(Action<,,,,,,,>),
				09 => typeof(Action<,,,,,,,,>),
				10 => typeof(Action<,,,,,,,,,>),
				11 => typeof(Action<,,,,,,,,,,>),
				12 => typeof(Action<,,,,,,,,,,,>),
				13 => typeof(Action<,,,,,,,,,,,,>),
				14 => typeof(Action<,,,,,,,,,,,,,>),
				15 => typeof(Action<,,,,,,,,,,,,,,>),
				16 => typeof(Action<,,,,,,,,,,,,,,,>),
				_ => throw new ArgumentOutOfRangeException(nameof(parameters), "Too many parameters for Action")
			};
		}

		var parameters = method.GetParameters();
		var returnType = method.ReturnType;
		Type func;
		if (returnType == typeof(void))
		{
			var types = parameters
				.Select(t => t.ParameterType)
				.ToArray();
			func = MakeActionParameter(types.Length);
			if (types.Length > 0)
				func = func.MakeGenericType(types);
		}
		else
		{
			var types = parameters
				.Select(t => t.ParameterType)
				.Append(returnType)
				.ToArray();
			func = MakeFuncParameter(types.Length - 1)
				.MakeGenericType(types);
		}
		return Delegate.CreateDelegate(func, instance, method);
	}

	/// <summary>
	/// Serializes a <see cref="JsValue"/> to a JSON string
	/// </summary>
	/// <param name="value">The value to serialize</param>
	/// <param name="engine">The optional engine to use</param>
	/// <returns>The JSON string</returns>
	public static string ToJson(this JsValue value, Engine? engine = null)
	{
		engine ??= new Engine();
		var ser = new Jint.Native.Json.JsonSerializer(engine);
		return ser.Serialize(value).AsString();
	}

	/// <summary>
	/// Determines if the type is a collection
	/// </summary>
	/// <param name="type">The type to check</param>
	/// <param name="valueType">The type of the data within the collection</param>
	/// <returns>Whether or not the type is a collection</returns>
	public static bool IsCollection(this Type type, [MaybeNullWhen(false)] out Type valueType)
	{
		var array = type.IsArray;
		if (type.IsArray)
		{
			valueType = type.GetElementType()!;
			return true;
		}

		var interType = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
		if (interType is null)
		{
			valueType = null;
			return false;
		}

		valueType = interType.GetGenericArguments()[0];
		return true;
	}

	/// <summary>
	/// Determines if the type is a dictionary
	/// </summary>
	/// <param name="type">The type to check</param>
	/// <param name="keyType">The type of the key</param>
	/// <param name="valueType">The type of the value</param>
	/// <returns>Whether or not the type is a dictionary</returns>
	public static bool IsDictionary(this Type type, [MaybeNullWhen(false)] out Type keyType, [MaybeNullWhen(false)] out Type valueType)
	{
		var dict = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
		if (!dict)
		{
			keyType = null;
			valueType = null;
			return false;
		}

		var args = type.GetGenericArguments();
		keyType = args[0];
		valueType = args[1];
		return true;
	}

	/// <summary>
	/// Determines if the type is a Task or ValueTask
	/// </summary>
	/// <param name="type">The type to check</param>
	/// <param name="valueType">The generic type of the task (if null - it's a Task or ValueTask without a generic)</param>
	/// <returns>Whether or not the type is a task</returns>
	public static bool IsTask(this Type type, out Type? valueType)
	{
		if (type == typeof(Task) || type == typeof(ValueTask))
		{
			valueType = null;
			return true;
		}

		Type[] types = [typeof(Task<>), typeof(ValueTask<>)];
		if (!type.IsGenericType)
		{
			valueType = null;
			return false;
		}

		var generic = type.GetGenericTypeDefinition();
		if (!types.Any(t => t == generic))
		{
			valueType = null;
			return false;
		}
		valueType = type.GetGenericArguments()[0];
		return true;
	}

	/// <summary>
	/// Uses reflection to get the field value from an object.
	/// </summary>
	/// <param name="instance">The instance object.</param>
	/// <param name="fieldName">The field's name which is to be fetched.</param>
	/// <returns>The field value from the object.</returns>
	public static object? GetPrivateFieldValue<T>(this T instance, string fieldName)
    {
        var bindFlags =
            BindingFlags.Instance | BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Static;
        var field = typeof(T).GetField(fieldName, bindFlags);
        return field?.GetValue(instance);
    }

    /// <summary>
    /// Uses reflection to get the field value from an object.
    /// </summary>
    /// <param name="instance">The instance object.</param>
    /// <param name="fieldName">The field's name which is to be fetched.</param>
    /// <returns>The field value from the object.</returns>
    public static TOut? GetPrivateFieldValue<T, TOut>(this T instance, string fieldName)
    {
        var value = instance.GetPrivateFieldValue(fieldName);
        return value is not null ? (TOut)value : default;
    }
}
