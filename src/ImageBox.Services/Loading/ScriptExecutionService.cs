namespace ImageBox.Services.Loading;

using Ast;
using Scripting;

public interface IScriptExecutionService
{

}

internal class ScriptExecutionService(
    IElementReflectionService _elements,
    ILogger<ScriptExecutionService> _logger) : IScriptExecutionService
{
    public const string VALUE_PROP = nameof(AstValue<string>.Value);
    public const string BIND_PROP = nameof(AstValue<string>.Bind);

    public async Task Execute(RenderContext context)
    {
        try
        {
            //Not runner? Skip it
            if (context.Runner is null) return;
            //Execute the script and get the result
            var result = await context.Runner.Execute(context);
            //Set the result to the root scope
            context.SetRootScope(result);
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

    public void HandleBind(RenderContext context, IElement instance, AstAttribute ast)
    {
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
    public void TraverseSpread(RenderContext context, IEnumerable<IElement>? elements)
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
