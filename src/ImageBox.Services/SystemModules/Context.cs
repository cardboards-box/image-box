using Jint.Native;

namespace ImageBox.Services.SystemModules;

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

    /// <summary>
    /// The percentage representing the placement of this frame in the gif
    /// </summary>
    public double progress => _context.Frame / (double)_context.TotalFrames;

    /// <summary>
    /// Pulsates between the min and max values using the given progress
    /// </summary>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <param name="progress">The progress value (will be fetched from context if null)</param>
    /// <param name="width">Whether or not the value represents a width or x axis value</param>
    /// <returns>The value between the given min and max</returns>
    public double pulsate(JsValue min, JsValue max, bool? width = null, double? progress = null)
    {
        var drawing = new Drawing(_context);
        var progressValue = progress ?? this.progress;

        var minValue = drawing.fromUnitString(min, width);
        var maxValue = drawing.fromUnitString(max, width);
        var output = progressValue < 0.5 ? progressValue : 1 - progressValue;
        return minValue + (maxValue - minValue) * output;
    }

    /// <summary>
    /// Executes the <see cref="pickOne(double, JsValue[])"/> method with the current progress
    /// </summary>
    /// <param name="items">The items to pick from</param>
    /// <returns>The item that was chosen</returns>
    public JsValue progressOne(params JsValue[] items)
    {
        return pickOne(progress, items);
    }

    /// <summary>
    /// Returns the value from the <paramref name="items"/> array that matches the current progress
    /// </summary>
    /// <param name="progress">The progress indicator</param>
    /// <param name="items">The items to choose from</param>
    /// <returns>The item that was chosen</returns>
    /// <remarks>If the index is invalid, the result will be capped to either the first or last item</remarks>
    public JsValue pickOne(double progress, params JsValue[] items)
    {
        var index = (int)Math.Max(Math.Min(items.Length * progress, items.Length - 1), 0);
        if (index < 0) return items.First();
        if (index >= items.Length) return items.Last();
        return items[index];
    }

    /// <summary>
    /// Gets the font from the context
    /// </summary>
    /// <param name="family">The optional name of the font family (defaults to the global font family)</param>
    /// <param name="style">The optional style of the font (defaults to <see cref="FontStyle.Regular"/>)</param>
    /// <returns>The font that fetched</returns>
    public Font font(string? family = null, string? style = null)
    {
        if (!Enum.TryParse<FontStyle>(style, true, out var fontStyle))
            fontStyle = FontStyle.Regular;

        family ??= _context.LastScope.Size.FontFamily;
        return _context.BoxContext.Fonts.GetFont(family, _context.LastScope, fontStyle);
    }
}
