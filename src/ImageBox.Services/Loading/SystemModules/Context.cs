namespace ImageBox.Services.Loading.SystemModules;

/// <summary>
/// Providers a way of easily accessing variables in the current context
/// </summary>
/// <param name="_context"></param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
public class Context(
    ContextFrame _context) : IScriptItem
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    /// <summary>
    /// Gets the value of a variable in the current context
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <returns>The value of the variable if it exists</returns>
    public object? get(string name)
    {
        //Get the stacks in reverse order so that the most recent scope is first
        var stack = _context.Stack
            .ToArray()
            .Reverse();
        //Iterate through each stack
        foreach (var scope in stack)
        {
            //If the variable exists in the current scope, return it
            if (scope.Variables.TryGetValue(name, out var value))
                return value;
        }
        //If the variable does not exist in any scope, return null
        return null;
    }
}
