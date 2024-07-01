namespace ImageBox.Services.Loading;

using Ast;
using Scripting;

/// <summary>
/// Handles executing and binding attributes in templates and scripts
/// </summary>
public interface IScriptExecutionService
{
    /// <summary>
    /// Executes the script and binds all of the top-level elements
    /// </summary>
    /// <param name="context">The render context to attach to</param>
    /// <returns></returns>
    /// <exception cref="RenderContextException">Thrown if something goes wrong during execution</exception>
    Task Execute(RenderContext context);

    /// <summary>
    /// Traverse through all of the elements in the context and handle binds or spreads
    /// </summary>
    /// <param name="context">The render context</param>
    /// <param name="elements">The elements to traverse through</param>
    void HandleAttributes(RenderContext context, IEnumerable<IElement>? elements = null);

    /// <summary>
    /// Generates a render scope context
    /// </summary>
    /// <param name="context">The full render context</param>
    /// <param name="parent">The target element</param>
    /// <param name="config">A method for configuring the scope</param>
    /// <returns>The context as a disposable object</returns>
    ScopeContext Scope(RenderContext context, IElement parent, Action<ScopeContext>? config = null);
}

internal class ScriptExecutionService(
    IElementReflectionService _elements,
    ILogger<ScriptExecutionService> _logger) : IScriptExecutionService
{
    public const string VALUE_PROP = nameof(AstValue<string>.Value);
    public const string BIND_PROP = nameof(AstValue<string>.Bind);

    /// <summary>
    /// Executes the script and binds all of the top-level elements
    /// </summary>
    /// <param name="context">The render context to attach to</param>
    /// <returns></returns>
    /// <exception cref="RenderContextException">Thrown if something goes wrong during execution</exception>
    public async Task Execute(RenderContext context)
    {
        try
        {
            //No runner? Skip it
            if (context.Runner is null) return;
            //Execute the script and get the result
            var result = await context.Runner.Execute(context);
            //Set the result to the root scope
            context.SetRootScope(result);
            //Find and bind all of the elements in the template root.
            HandleAttributes(context);
        }
        catch (RenderContextException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing element script");
            throw new RenderContextException("Error occurred while executing element script", ex, context.Context);
        }
    }

    /// <summary>
    /// Generates a render scope context and binds it to the parent
    /// </summary>
    /// <param name="context">The full render context</param>
    /// <param name="parent">The target element</param>
    /// <param name="config">A method for configuring the scope</param>
    /// <returns>The context as a disposable object</returns>
    public ScopeContext Scope(RenderContext context, IElement parent, Action<ScopeContext>? config = null)
    {
        var output = new ScopeContext(context, this, parent);
        config?.Invoke(output);
        return output.Bind();
    }

    /// <summary>
    /// Handles any spread attributes on the element
    /// </summary>
    /// <param name="context">The context to bind</param>
    /// <param name="instance">The element to bind to</param>
    /// <param name="ast">The AST attribute that is being bound</param>
    public void HandleSpread(RenderContext context, IElement instance, AstAttribute ast)
    {
        //Ensure there is an evaluator
        ast.Cache ??= new ExpressionEvaluator(ast.Name);
        //Get the evaluator and ensure the type
        if (ast.Cache is not ExpressionEvaluator evaluator) return;
        //Bind the evaluator with the current context
        context.BindTo(evaluator);
        //Evaluate the expression
        var value = evaluator.Evaluate();
        //If the value is null, skip it
        if (value is null) return;
        //Iterate through all the properties
        foreach (var (key, val) in value.Enumerator())
        {
            //Find the property in the element
            var attribute = GetProperty(context, instance, key);
            //Couldn't find it? well damn.
            if (attribute is null) continue;
            //The properties parent
            object? target = instance;
            //The property to target
            var property = attribute.Type;
            //Check if the attribute is an AstValue<>
            if (attribute.IsBindable)
            {
                //Get the value of the target property
                target = property.GetValue(instance);
                //Get the property type of the value
                property = target?.GetType().GetProperty(VALUE_PROP);
                //This shouldn't be happening, but ok.
                if (property is null || target is null) continue;
            }
            //Bind the value of the property
            _elements.TypeCastBind(property, target, val);
        }
    }

    /// <summary>
    /// Handles any bind attributes on the element
    /// </summary>
    /// <param name="context">The context to bind</param>
    /// <param name="instance">The element to bind to</param>
    /// <param name="ast">The AST attribute that is being bound</param>
    /// <exception cref="RenderContextException">Thrown if the reflection requests fail</exception>
    public void HandleBind(RenderContext context, IElement instance, AstAttribute ast)
    {
        //Get the property type and reflection data
        var property = GetProperty(context, instance, ast.Name);
        if (property is null) return;
        //Validate the property
        if (!property.IsBindable)
            throw new RenderContextException(
                $"AST attribute is marked as bind, but the C# property isn't bindable: {ast.Name}",
                context.Context,
                instance.Context);
        //Get the AstValue<> instance on the element
        var astValue = property.Type.GetValue(instance) 
            ?? throw new RenderContextException(
                $"AST attribute is marked as bind, but the C# property is null: {ast.Name}",
                context.Context,
                instance.Context);
        //Get the expression value from the AstValue<>
        var bindValue = astValue.GetType()
            .GetProperty(BIND_PROP)?
            .GetValue(astValue);
        //Get the value property from the AstValue<>
        var valueProp = astValue.GetType()
            .GetProperty(VALUE_PROP);
        //Validate the expression and value properties
        if (bindValue is not ExpressionEvaluator expression || valueProp is null)
            throw new RenderContextException(
                $"AST attribute is marked as bind, but the C# property is missing bind property: {ast.Name}",
                context.Context,
                instance.Context);
        //Set the context for the expression
        context.BindTo(expression);
        //Get the value of the expression
        var value = expression.Evaluate();
        //Set the value of the expression 
        _elements.TypeCastBind(valueProp, astValue, value);
    }

    /// <summary>
    /// Get the reflected attribute property from the element
    /// </summary>
    /// <param name="context">The render context</param>
    /// <param name="element">The target element</param>
    /// <param name="name">The name of the attribute to fetch</param>
    /// <returns>The reflected property data</returns>
    /// <exception cref="RenderContextException">Thrown if multiple attribute properties exist</exception>
    public static ReflectedAttribute? GetProperty(RenderContext context, IElement element, string name)
    {
        var props = element.Reflected!.Props
            .Where(t => t.Attributes.Any(a => 
                a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
        if (props.Length > 1)
            throw new RenderContextException(
                $"Ambiguous property '{name}' found in element", 
                context.Context, element.Context);
        return props.FirstOrDefault();
    }

    /// <summary>
    /// Handle any bind or spread attributes on the element
    /// </summary>
    /// <param name="context">The render context</param>
    /// <param name="instance">The element to handle the attributes of</param>
    public void BindAttributes(RenderContext context, IElement instance)
    {
        //If there is no reflected context or AST context, skip it
        if (instance.Context is null || instance.Reflected is null) return;
        //Iterate through all of the AST attributes
        foreach (var attr in instance.Context.Attributes)
        {
            //Handle the spread attributes
            if (attr.Type == AstAttributeType.Spread)
            {
                HandleSpread(context, instance, attr);
                continue;
            }
            //Handle the bind attributes
            if (attr.Type == AstAttributeType.Bind)
            {
                HandleBind(context, instance, attr);
                continue;
            }
        }
    }

    /// <summary>
    /// Traverse through all of the elements in the context and handle binds or spreads
    /// </summary>
    /// <param name="context">The render context</param>
    /// <param name="elements">The elements to traverse through</param>
    public void HandleAttributes(RenderContext context, IEnumerable<IElement>? elements = null)
    {
        //Get the current element list to iterate (mostly for recursion)
        elements ??= context.Template.Children;
        //Iterate through each element
        foreach (var element in elements)
        {
            //Bind any spread or bind attributes
            BindAttributes(context, element);
        }
    }
}