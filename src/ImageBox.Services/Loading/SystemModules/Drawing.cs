namespace ImageBox.Services.Loading.SystemModules;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
internal class Drawing(
    RenderContext _context)
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    public double UnitContext(string value, SizeContext? context, bool? isWidth)
    {
        context ??= _context.Size;
        return SizeUnit.Parse(value).Pixels(context, isWidth);
    }

    public double unit(string value) => UnitContext(value, null, null);

    public double right(string value)
    {
        var ctx = _context.Size;
        var size = UnitContext(value, ctx, true);
        return ctx.Root.Width - size;
    }

    public double left(string value) => UnitContext(value, null, true);

    public double top(string value) => UnitContext(value, null, false);

    public double bottom(string value)
    {
        var ctx = _context.Size;
        var size = UnitContext(value, ctx, false);
        return ctx.Root.Height - size;
    }
}