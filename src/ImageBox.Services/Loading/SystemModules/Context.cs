namespace ImageBox.Services.Loading.SystemModules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
internal class Context(
    RenderContext _context)
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

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
