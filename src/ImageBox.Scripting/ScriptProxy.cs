using Jint.Runtime.Modules;
using System.Dynamic;

namespace ImageBox.Scripting;

/// <summary>
/// A dynamic proxy for script objects that allows access to properties and methods
/// </summary>
/// <param name="_instance">The object instance to proxy</param>
public class ScriptProxy(object _instance) : DynamicObject, IConvertible
{
	private readonly Dictionary<string, PropertyInfo> _properties = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, Delegate> _methods = new(StringComparer.InvariantCultureIgnoreCase);
	private bool _isInitialized = false;

	private void FillCache()
	{
		if (_isInitialized) return;

		_isInitialized = true;
		var type = _instance.GetType();
		foreach (var prop in type.GetProperties())
		{
			var name = prop.GetCustomAttribute<ModuleExportAttribute>()?.Name?.ForceNull() ?? prop.Name;
			_properties[name] = prop;
		}

		foreach (var method in type.GetMethods())
		{
			var name = method.GetCustomAttribute<ModuleExportAttribute>()?.Name?.ForceNull() ?? method.Name;
			_methods[name] = method.ToDelegate(_instance);
		}
	}

	/// <inheritdoc />
	public override bool TryGetMember(GetMemberBinder binder, out object? result)
	{
		FillCache();
		if (_properties.TryGetValue(binder.Name, out var property))
		{
			result = property.GetValue(_instance);
			return true;
		}

		if (_methods.TryGetValue(binder.Name, out var method))
		{
			result = method;
			return true;
		}

		result = null;
		return false;
	}

	/// <inheritdoc />
	public override bool TrySetMember(SetMemberBinder binder, object? value)
	{
		FillCache();
		if (!_properties.TryGetValue(binder.Name, out var property))
			return false;

		property.SetValue(_instance, value);
		return true;
	}

	/// <inheritdoc />
	public TypeCode GetTypeCode()
	{
		return Convert.GetTypeCode(_instance);
	}

	/// <inheritdoc />
	public bool ToBoolean(IFormatProvider? provider)
	{
		return Convert.ToBoolean(_instance, provider);
	}

	/// <inheritdoc />
	public byte ToByte(IFormatProvider? provider)
	{
		return Convert.ToByte(_instance, provider);
	}

	/// <inheritdoc />
	public char ToChar(IFormatProvider? provider)
	{
		return Convert.ToChar(_instance, provider);
	}

	/// <inheritdoc />
	public DateTime ToDateTime(IFormatProvider? provider)
	{
		return Convert.ToDateTime(_instance, provider);
	}

	/// <inheritdoc />
	public decimal ToDecimal(IFormatProvider? provider)
	{
		return Convert.ToDecimal(_instance, provider);
	}

	/// <inheritdoc />
	public double ToDouble(IFormatProvider? provider)
	{
		return Convert.ToDouble(_instance, provider);
	}

	/// <inheritdoc />
	public short ToInt16(IFormatProvider? provider)
	{
		return Convert.ToInt16(_instance, provider);
	}

	/// <inheritdoc />
	public int ToInt32(IFormatProvider? provider)
	{
		return Convert.ToInt32(_instance, provider);
	}

	/// <inheritdoc />
	public long ToInt64(IFormatProvider? provider)
	{
		return Convert.ToInt64(_instance, provider);
	}

	/// <inheritdoc />
	public sbyte ToSByte(IFormatProvider? provider)
	{
		return Convert.ToSByte(_instance, provider);
	}

	/// <inheritdoc />
	public float ToSingle(IFormatProvider? provider)
	{
		return Convert.ToSingle(_instance, provider);
	}

	/// <inheritdoc />
	public string ToString(IFormatProvider? provider)
	{
		return Convert.ToString(_instance, provider)!;
	}

	/// <inheritdoc />
	public object ToType(Type conversionType, IFormatProvider? provider)
	{
		if (_instance is IConvertible conv)
			return conv.ToType(conversionType, provider);

		return _instance;
	}

	/// <inheritdoc />
	public ushort ToUInt16(IFormatProvider? provider)
	{
		return Convert.ToUInt16(_instance, provider);
	}

	/// <inheritdoc />
	public uint ToUInt32(IFormatProvider? provider)
	{
		return Convert.ToUInt32(_instance, provider);
	}

	/// <inheritdoc />
	public ulong ToUInt64(IFormatProvider? provider)
	{
		return Convert.ToUInt64(_instance, provider);
	}
}
