using SixLabors.ImageSharp;
using System.Numerics;

namespace ImageBox.Drawing;

/// <summary>
/// Helpful ImageSharp extensions
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets a rectangle from a size context
    /// </summary>
    /// <param name="size">The size context</param>
    /// <returns>The rectangle</returns>
    public static Rectangle GetRectangle(this SizeContext size) => new(size.X, size.Y, size.Width, size.Height);

    /// <summary>
    /// Gets the top left corner of a rectangle
    /// </summary>
    /// <param name="rectangle">The rectangle</param>
    /// <returns>The top left corner as a vector</returns>
    public static Vector2 TopLeft(this Rectangle rectangle) => new(rectangle.Left, rectangle.Top);

    /// <summary>
    /// Gets the center of the rectangle
    /// </summary>
    /// <param name="rectangle">The rectangle</param>
    /// <returns>The center point as a vector</returns>
    public static Vector2 Center(this Rectangle rectangle) => new(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);

    /// <summary>
    /// Get the color from a string
    /// </summary>
    /// <param name="color">The color</param>
    /// <param name="default">The default color to use if none is provided</param>
    /// <returns>The parsed color</returns>
    public static Color ParseColor(this string? color, Color? @default = null)
    {
        if (string.IsNullOrEmpty(color))
            return @default ?? Color.Transparent;

        return Color.Parse(color);
    }

    /// <summary>
    /// Debounce an action
    /// </summary>
    /// <param name="func">The function to debounce</param>
    /// <param name="milliseconds">The number of milliseconds to wait before running</param>
    /// <returns>The action to execute</returns>
    public static Action Debounce(this Action func, int milliseconds = 300)
    {
        var last = 0;
        return () =>
        {
            var current = Interlocked.Increment(ref last);
            Task.Delay(milliseconds).ContinueWith(task =>
            {
                if (current == last) func();
                task.Dispose();
            });
        };
    }

    /// <summary>
    /// Debounce an action
    /// </summary>
    /// <typeparam name="T">The type of argument</typeparam>
    /// <param name="func">The function to debounce</param>
    /// <param name="milliseconds">The number of milliseconds to wait before running</param>
    /// <returns>The action to execute</returns>
    public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
    {
        var last = 0;
        return (arg) =>
        {
            var current = Interlocked.Increment(ref last);
            Task.Delay(milliseconds).ContinueWith(task =>
            {
                if (current == last) func(arg);
                task.Dispose();
            });
        };
    }

}
