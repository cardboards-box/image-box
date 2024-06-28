using Jint;
using Jint.Native;

namespace ImageBox.Scripting;

/// <summary>
/// Helpful extensions for scripting
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Iterates through the properties of the given JsValue
    /// </summary>
    /// <param name="value">The value to iterate through</param>
    /// <returns>All of the properties</returns>
    public static IEnumerable<KeyValuePair<string, JsValue>> Enumerator(this JsValue? value)
    {
        if (value is null ||
            value.IsUndefined() ||
            value.IsNull() ||
            value.IsArray() ||
            !value.IsObject()) 
            yield break;

        var props = value.AsObject().GetOwnProperties();
        foreach (var item in props)
        {
            var key = item.Key.ToString();
            var val = item.Value.Value;
            yield return new KeyValuePair<string, JsValue>(key, val);
        }
    }

    /// <summary>
    /// Iterates through the given JsValue and returns a dictionary of properties
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, object?> ToDictionary(this JsValue? value)
    {
        var output = new Dictionary<string, object?>();
        value?.AppendTo(output);
        return output;
    }

    /// <summary>
    /// Appends all of the items in the JsValue to the given dictionary
    /// </summary>
    /// <param name="value"></param>
    /// <param name="output"></param>
    public static void AppendTo(this JsValue? value, Dictionary<string, object?> output)
    {
        foreach (var (key, val) in value.Enumerator())
            if (!output.TryAdd(key, val))
                output[key] = val;
    }
}
