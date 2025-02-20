namespace ImageBox.Elements;

using Ast;

/// <summary>
/// A utility for resolving <see cref="IElement"/>s from <see cref="AstElement"/>s
/// </summary>
public interface IElementReflectionService
{
    /// <summary>
    /// Iterates through all <see cref="AstElement"/>s and gets the associated <see cref="IElement"/>
    /// </summary>
    /// <param name="elements">The elements to iterate through</param>
    /// <param name="skipChildren">Whether or not to skip binding the children</param>
    /// <param name="parent">The parent element</param>
    /// <returns>All of the <see cref="IElement"/> instances</returns>
    /// <exception cref="MissingMemberException">Thrown if the config is set to throw errors and an element instance or attribute instance is missing</exception>
    IEnumerable<IElement> BindTemplates(IEnumerable<AstElement> elements, bool skipChildren, IElement? parent);

    /// <summary>
    /// Converts the given string to the property type and sets it's value
    /// </summary>
    /// <param name="property">The property to bind to</param>
    /// <param name="instance">The object to set the value on</param>
    /// <param name="value">The string to set the value to</param>
    void TypeCastBind(PropertyInfo property, object instance, object? value);

    /// <summary>
    /// Gets all concrete types that implement the given type
    /// </summary>
    /// <param name="type">The type that should be implemented</param>
    /// <returns>All of the concrete types</returns>
    IEnumerable<Type> GetAllOfType(Type type);

    /// <summary>
    /// Gets all concrete types that implement the given type
    /// </summary>
    /// <typeparam name="T">The type that should be implemented</typeparam>
    /// <returns>All of the concrete types</returns>
    IEnumerable<Type> GetAllOfType<T>();
}
