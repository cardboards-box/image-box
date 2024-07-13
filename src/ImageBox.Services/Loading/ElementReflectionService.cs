using System.Collections;

namespace ImageBox.Services.Loading;

using Ast;
using Jint.Native;
using Scripting;

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
    /// <returns>All of the <see cref="IElement"/> instances</returns>
    /// <exception cref="MissingMemberException">Thrown if the config is set to throw errors and an element instance or attribute instance is missing</exception>
    IEnumerable<IElement> BindTemplates(IEnumerable<AstElement> elements, bool skipChildren);

    /// <summary>
    /// Converts the given string to the property type and sets it's value
    /// </summary>
    /// <param name="property">The property to bind to</param>
    /// <param name="instance">The object to set the value on</param>
    /// <param name="value">The string to set the value to</param>
    void TypeCastBind(PropertyInfo property, object instance, object? value);
}

internal class ElementReflectionService(
    IServiceProvider _services,
    IServiceConfig _config,
    ILogger<ElementReflectionService> _logger) : IElementReflectionService
{
    private ReflectedElement[]? _elements;

    /// <summary>
    /// This method is purely here because C#'s type system is annoying
    /// </summary>
    /// <returns>Gets all of the assemblies in the current domain</returns>
    public static IEnumerable<Assembly> GetAllAssemblies()
    {
        //Create a collection to store all of the loaded assemblies
        var list = new HashSet<string>();
        //Create a stack of the assemblies to iterate through
        var stack = new Stack<Assembly>();
        //Add known assemblies to the stack
        var entry = Assembly.GetEntryAssembly();
        if (entry is not null)
            stack.Push(entry);
        entry = Assembly.GetCallingAssembly();
        if (entry is not null)
            stack.Push(entry);
        entry = Assembly.GetExecutingAssembly();
        if (entry is not null)
            stack.Push(entry);
        //Add all of the assemblies in the current domain to the stack
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            stack.Push(asm);
        //Iterate through the stack
        do
        {
            //Get the current assembly
            var asm = stack.Pop();
            var name = asm.FullName;
            //Skip the assembly if it's already been loaded
            if (string.IsNullOrEmpty(name) ||
                list.Contains(name)) 
                continue;
            //Return the assembly out
            yield return asm;
            //Add the assembly to the list
            list.Add(name);
            //Iterate through all of the referenced assemblies
            foreach (var reference in asm.GetReferencedAssemblies())
            {
                //If the assembly has already been processed, skip it
                if (list.Contains(reference.FullName)) continue;
                //Add the assembly to the stack
                stack.Push(Assembly.Load(reference));
            }
        }
        while (stack.Count > 0);
    }

    /// <summary>
    /// Get all of the possible instances of <see cref="IElement"/>
    /// </summary>
    /// <returns>The possible instances of <see cref="IElement"/></returns>
    /// <exception cref="InvalidOperationException">Thrown if the type configuration is bad</exception>
    public ReflectedElement[] AllElementTypes()
    {
        //Return from the cache if possible
        if (_elements is not null) return _elements;
        //Some type instances for checking
        var elementType = typeof(IElement);
        var parentType = typeof(IParentElement);
        var valueType = typeof(IValueElement);
        var astValueType = typeof(AstValue<>);
        //Get all of the concrete types matching IElement
        var types = GetAllAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(elementType.IsAssignableFrom)
            .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract);
        //A collection of all of the reflected elements
        var output = new List<ReflectedElement>();
        //Iterate through all of the types
        foreach (var type in types)
        {
            //Get the template attribute configuration
            var elemAttr = type.GetCustomAttributes<AstElementAttribute>().ToArray();
            //Skip type if no attribute is found
            if (elemAttr is null || elemAttr.Length == 0) continue;
            //Get the first constructor of the type (for dependency injection)
            var constructor = type.GetConstructors().FirstOrDefault()?.GetParameters() ?? [];
            //Get all of the properties on the type
            var properties = type.GetProperties();
            //Storage for figuring out the children types
            var childType = AstElementType.Empty;
            PropertyInfo? child = null;
            //Element has children, get the property and type
            if (parentType.IsAssignableFrom(type))
            {
                childType = AstElementType.Children;
                child = properties.First(t => t.Name == nameof(IParentElement.Children));
            }
            //Element has a value property, get the property and type
            if (valueType.IsAssignableFrom(type))
            {
                //Cannot be both a parent element and value element
                if (childType != AstElementType.Empty)
                {
                    _logger.LogWarning("{type} cannot implement both {IParentElement} and {IValueElement}", type.FullName, nameof(IParentElement), nameof(IValueElement));
                    if (_config.Parser.Errors.ElementInvalidChild)
                        throw new InvalidOperationException(
                            $"{type.FullName} cannot implement both {nameof(IParentElement)} and {nameof(IValueElement)}");
                    continue;
                }
                childType = AstElementType.Text;
                child = properties.First(t => t.Name == nameof(IValueElement.Value));
            }
            //Storage for properties and their attributes
            var children = new List<ReflectedAttribute>();
            //Iterate through all of the properties to get their attributes
            foreach (var prop in properties)
            {
                //Get the potential attributes
                var propAttr = prop.GetCustomAttributes<AstAttributeAttribute>().ToArray();
                //No attributes, skip the property, it's probably for something else
                if (propAttr is null || propAttr.Length == 0) continue;

                //Does the property type have a generic?
                if (!prop.PropertyType.IsGenericType)
                {
                    //No, just add the children with no generic types
                    children.Add(new ReflectedAttribute(prop, propAttr, false, null));
                    continue;
                }
                //Get the generic type definition and check if it's an AstValue<>
                var genericType = prop.PropertyType.GetGenericTypeDefinition();
                if (genericType == astValueType)
                {
                    //Get the generic type for the AstValue<>
                    var bindType = prop.PropertyType.GetGenericArguments().First();
                    //Add it with the type mapping
                    children.Add(new ReflectedAttribute(
                        prop,
                        propAttr,
                        true,
                        bindType));
                    continue;
                }
                //Not an AstValue<> so skip it
                children.Add(new ReflectedAttribute(prop, propAttr, false, null));
            }
            //Add the entire reflection info to storage
            output.Add(new ReflectedElement(
                type,
                elemAttr,
                childType,
                child,
                constructor,
                [.. children]));
        }
        //Cache and store the output
        return _elements = [.. output];
    }

    /// <summary>
    /// Get and bind an instance of the type
    /// </summary>
    /// <param name="element">The reflection information for the element</param>
    /// <returns>The instance of the <see cref="IElement"/></returns>
    public IElement? GenerateInstance(ReflectedElement element)
    {
        //If the element is null, skip it.
        if (element is null) return null;
        //Get dependency injection instances of the constructor parameters
        var constructors = element.Constructors
            .Select(t => _services.GetRequiredService(t.ParameterType))
            .ToArray();
        //Create the instance with the parameters
        var result = Activator.CreateInstance(element.Type, constructors);
        //Null check the activated instance
        return result is null ? null : (IElement)result;
    }

    /// <summary>
    /// Converts the given string to the property type and sets it's value
    /// </summary>
    /// <param name="property">The property to bind to</param>
    /// <param name="instance">The object to set the value on</param>
    /// <param name="value">The string to set the value to</param>
    public void TypeCastBind(PropertyInfo property, object instance, object? value)
    {
        void Set(object? value) => property.SetValue(instance, value);
        //Check if the property is nullable
        var notNullType = Nullable.GetUnderlyingType(property.PropertyType);
        var isNullable = notNullType is not null;
        //Get the non-nullable type for conversion
        var nonNullable = notNullType ?? property.PropertyType;
        //Null value? Ignore it and continue
        if (value is null)
        {
            if (isNullable)
                Set(null);
            return;
        }
        var valueType = value.GetType();
        //If the property types match, just set the value
        if (property.PropertyType == valueType ||
            nonNullable == valueType)
        {
            Set(value);
            return;
        }
        if (nonNullable == typeof(object))
        {
            Set(value);
            return;
        }
        //Check the properties and set the value if there's a match
        if (nonNullable == typeof(SizeUnit))
        {
            Set(SizeUnit.Parse(value.ToString() ?? string.Empty));
            return;
        }
        if (nonNullable == typeof(TimeUnit))
        {
            Set(TimeUnit.Parse(value.ToString() ?? string.Empty));
            return;
        }
        if (nonNullable == typeof(IOPath))
        {
            Set(new IOPath(value.ToString() ?? string.Empty));
            return;
        }
        //Try bind object arrays (specifically for arrays)
        if (nonNullable == typeof(object?[]))
        {
            //If the value is an array, enumerate it and set the values
            if (typeof(IEnumerable).IsAssignableFrom(valueType))
            {
                var collection = (IEnumerable)value;
                var output = new List<object>();
                foreach (var item in collection)
                    output.Add(item);

                Set(output.ToArray());
                return;
            }
            //If it's not an array, just set the value
            Set(new object?[] { value });
            return;
        }
        object? converted;
        try
        {
            //Try to convert the raw value
            converted = Convert.ChangeType(value, nonNullable);
            //set the converted value
            Set(converted);
        }
        catch
        {
            //If the type is already a string, throw an exception
            if (value is string) throw;
            //If it fails, try to convert the string version of the value
            TypeCastBind(property, instance, value.ToString() ?? string.Empty);
        }
    }

    /// <summary>
    /// Binds the AST attribute property
    /// </summary>
    /// <param name="element">The instance of the <see cref="IElement"/> to bind to</param>
    /// <param name="attribute">The information about the property to set</param>
    /// <param name="ast">The AST value to bind from</param>
    public void BindProperty(IElement element, ReflectedAttribute attribute, AstAttribute ast)
    {
        //Skip spread syntax properties
        if (ast.Type == AstAttributeType.Spread) return;
        //Property names for reflection & value expansions
        var valuePropName = nameof(AstValue<string>.Value);
        var bindPropName = nameof(AstValue<string>.Bind);
        var contextPropName = nameof(AstValue<string>.Context);
        var (type, _, isBindable, _) = attribute;
        var actualValue = ast.Type == AstAttributeType.BooleanTrue ? "true" : ast.Value;
        //If the property isn't bindable, handle default types
        if (!isBindable)
        {
            //Ast is bind but property isn't bindable - Error
            if (ast.Type != AstAttributeType.Bind)
            {
                //Set the property value out-right
                TypeCastBind(type, element, actualValue);
                return;
            }
            //Throw an error if it's configured to
            _logger.LogWarning("Cannot bind value as the target property isn't an AstValue<T>: " +
                    "{astName}={actualValue} >> {Context}", ast.Name, actualValue, element.Context?.ExceptionString());
            if (_config.Parser.Errors.AttributeBindInvalid)
                throw new InvalidOperationException(
                    "Cannot bind value as the target property isn't an AstValue<T>: " +
                    $"{ast.Name}={actualValue} >> {element.Context?.ExceptionString()}");
            return;
        }
        //Get an instance of the value 
        var astValue = type.GetValue(element)
            ?? Activator.CreateInstance(type.PropertyType);
        //Couldn't create ast value so breakout
        if (astValue is null) return;
        //Get the full type of the AstValue{T}
        var astType = astValue.GetType();
        //Get and set the context property of the AstValue{T}
        var contextProp = astType.GetProperty(contextPropName);
        contextProp?.SetValue(astValue, ast);
        //The value from the ast is a bind, so set the expression
        if (ast.Type == AstAttributeType.Bind)
        {
            //No expression found, skip it.
            if (string.IsNullOrEmpty(actualValue)) return;
            //Create the expression
            var exp = new ExpressionEvaluator(actualValue);
            //Get the bind property name
            var prop = astType.GetProperty(bindPropName);
            if (prop is null) return;
            //Set the bind value
            prop.SetValue(astValue, exp);
            type.SetValue(element, astValue);
            return;
        }
        //Set the value of the expression if it's a bindable value but not an ast-bind
        var valueProp = astType.GetProperty(valuePropName);
        if (valueProp is null) return;
        TypeCastBind(valueProp, astValue, actualValue);
        type.SetValue(element, astValue);
    }

    /// <summary>
    /// Creates an element instance from the given AST element
    /// </summary>
    /// <param name="type">The reflection target for the instance</param>
    /// <param name="element">The AST element to create the instance from</param>
    /// <param name="skipChildren">Whether or not to skip binding the children</param>
    /// <returns>The instance of the element</returns>
    /// <exception cref="InvalidOperationException">Thrown if the config is set to throw errors and an element instance or attribute instance is missing</exception>
    public IElement? BindInstance(ReflectedElement type, AstElement element, bool skipChildren)
    {
        //Generate the instance from the reflected type
        var instance = GenerateInstance(type);
        if (instance is null) return null;
        //Set the contexts for the renderer
        instance.Reflected = type;
        instance.Context = element;
        //Find and set any child elements on this template
        if (type.ChildType == AstElementType.Children &&
            element.Children.Length > 0 &&
            !skipChildren)
        {
            var children = BindTemplates(element.Children, skipChildren).ToArray();
            type.ChildProperty!.SetValue(instance, children);
        }
        //Find and set the template value
        if (type.ChildType == AstElementType.Text &&
            !string.IsNullOrWhiteSpace(element.Value))
        {
            var text = element.Value;
            type.ChildProperty!.SetValue(instance, text);
        }
        //Iterate through all of the AST attributes
        foreach (var attr in element.Attributes)
        {
            //Find any matching reflected attributes
            var props = type.Props
                .Where(t => t.Attributes.Any(a =>
                    a.Name.Equals(attr.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
            //No attribute? No problem; skip it. Validation can be done during rendering.
            if (props.Length == 0) continue;
            //More than one attribute? Validate and throw error if necessary
            if (props.Length > 1)
            {
                _logger.LogWarning("More than one properties match: {type}::{attr} >> {exStr}",
                    type.Type.FullName, attr.Name, element.ExceptionString());
                if (_config.Parser.Errors.AttributeMoreThanOne)
                    throw new InvalidOperationException(
                        $"More than one properties match: {type.Type.FullName}::" +
                        $"{attr.Name} >> {element.ExceptionString()}");
                continue;
            }
            //Get the property and bind it's value
            BindProperty(instance, props.First(), attr);
        }
        //Return the instance
        return instance;
    }

    /// <summary>
    /// Iterates through all <see cref="AstElement"/>s and gets the associated <see cref="IElement"/>
    /// </summary>
    /// <param name="elements">The elements to iterate through</param>
    /// <param name="skipChildren">Whether or not to skip binding the children</param>
    /// <returns>All of the <see cref="IElement"/> instances</returns>
    /// <exception cref="MissingMemberException">Thrown if the config is set to throw errors and an element instance or attribute instance is missing</exception>
    public IEnumerable<IElement> BindTemplates(IEnumerable<AstElement> elements, bool skipChildren)
    {
        //Get all of the possible instances of IElement
        var types = AllElementTypes();
        //Iterate through all of the AST elements
        foreach (var element in elements)
        {
            //Get the exception string for logging purposes
            var exStr = element.ExceptionString();
            //Find all of the matching reflected elements
            var foundTypes = types
                .Where(t => t.Attribute.Any(t => t.Tag == element.Tag))
                .ToArray();

            //No matches found
            if (foundTypes.Length == 0)
            {
                _logger.LogWarning("Could not find element type for: {exStr}", exStr);
                if (_config.Parser.Errors.ElementNotFound)
                    throw new MissingMemberException($"Could not find element type for: {exStr}");
                continue;
            }
            //More than one match found
            if (foundTypes.Length > 1)
            {
                _logger.LogWarning("Multiple element types found for: {exStr}", exStr);
                if (_config.Parser.Errors.ElementMoreThanOne)
                    throw new MissingMemberException($"Multiple element types found for: {exStr}");
                continue;
            }
            //The correct match
            var type = foundTypes.First();
            //The instance of the IElement with all of it's properties bound
            var instance = BindInstance(type, element, skipChildren);
            //Filter missing instances
            if (instance is null)
            {
                _logger.LogWarning("Could not create instance of type: {type} >> {exStr}", type.Type.FullName, exStr);
                if (_config.Parser.Errors.InvalidInstance)
                    throw new MissingMemberException($"Could not create instance of type: {type.Type.FullName} >> {exStr}");
                continue;
            }
            //Return the instance
            yield return instance;
        }
    }
}
