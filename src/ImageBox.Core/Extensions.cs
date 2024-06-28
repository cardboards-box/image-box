namespace ImageBox.Core;

/// <summary>
/// A collection of extensions
/// </summary>
public static class Extensions
{
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
