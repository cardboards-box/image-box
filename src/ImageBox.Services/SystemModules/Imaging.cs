using ImageBox.Drawing;

namespace ImageBox.Services.SystemModules;

/// <summary>
/// Providers a set of functions related to image processing
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
public class Imaging(IFileResolverService _resolver, LoadedAst _ast) : IScriptItem
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    /// <summary>
    /// Loads an image from the given path
    /// </summary>
    /// <param name="path">The path to load the image from</param>
    /// <returns>The loaded image</returns>
    public Image load(string path)
    {
        var task = _resolver.Fetch(path, _ast.WorkingDirectory);
        var (stream, _, _, _) = task.Result;
        return Image.Load(stream);
    }

    /// <summary>
    /// Greyscales the given image by the amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image grayscale(Image image, float amount)
    {
        image.Mutate(x => x.Grayscale(amount));
        return image;
    }

    /// <summary>
    /// Blurs the given image by the amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image blur(Image image, float amount)
    {
        image.Mutate(x => x.GaussianBlur(amount));
        return image;
    }

    /// <summary>
    /// Sharpens the given image by the amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image sharpen(Image image, float amount)
    {
        image.Mutate(x => x.GaussianSharpen(amount));
        return image;
    }

    /// <summary>
    /// Converts the image to black and white
    /// </summary>
    /// <param name="image">The image</param>
    /// <returns>The image</returns>
    public Image blackWhite(Image image)
    {
        image.Mutate(x => x.BlackWhite());
        return image;
    }

    /// <summary>
    /// Alters the brightness of the given image by the amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image brightness(Image image, float amount)
    {
        image.Mutate(x => x.Brightness(amount));
        return image;
    }

    /// <summary>
    /// Applies the given colorblindness simulator to the image
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="mode">The <see cref="ColorBlindnessMode"/> to apply</param>
    /// <returns>The image</returns>
    public Image colorBlind(Image image, string mode)
    {
        if (!Enum.TryParse<ColorBlindnessMode>(mode, true, out var m))
        {
            var options = string.Join(", ", ColorBlindnessMode.Protanopia.AllFlags().Select(t => t.ToString()));
            throw new RenderContextException($"Invalid colorblind mode. Valid options: {options}", _ast);
        }

        image.Mutate(x => x.ColorBlindness(m));
        return image;
    }

    /// <summary>
    /// Alters the contrast of the given image by the amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image contrast(Image image, float amount)
    {
        image.Mutate(x => x.Contrast(amount));
        return image;
    }

    /// <summary>
    /// Flips the given image
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="mode">The <see cref="FlipMode"/> to apply</param>
    /// <returns>The image</returns>
    public Image flip(Image image, string mode)
    {
        if (!Enum.TryParse<FlipMode>(mode, true, out var m))
        {
            var options = string.Join(", ", FlipMode.Horizontal.AllFlags().Select(t => t.ToString()));
            throw new RenderContextException($"Invalid flip mode. Valid options: {options}", _ast);
        }

        image.Mutate(x => x.Flip(m));
        return image;
    }

    /// <summary>
    /// Alters the colors of the image recreating an oil painting effect.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="levels">The number of intensity levels. Higher values result in a broader range of color intensities forming part of the result image.</param>
    /// <param name="brushSize">The number of neighboring pixels used in calculating each individual pixel value.</param>
    /// <returns>The image</returns>
    public Image oilPaint(Image image, int levels = 10, int brushSize = 15)
    {
        image.Mutate(x => x.OilPaint(levels, brushSize));
        return image;
    }

    /// <summary>
    /// Pixelates the image by the given pixel size
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="size">The size of the pixels</param>
    /// <returns>The image</returns>
    public Image pixelate(Image image, int size)
    {
        image.Mutate(x => x.Pixelate(size));
        return image;
    }

    /// <summary>
    /// Applies the polaroid effect to the image
    /// </summary>
    /// <param name="image">The image</param>
    /// <returns>The image</returns>
    public Image polaroid(Image image)
    {
        image.Mutate(x => x.Polaroid());
        return image;
    }

    /// <summary>
    /// Resizes the image to the given width and height
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="width">The width of the image</param>
    /// <param name="height">The height of the image</param>
    /// <returns>The image</returns>
    public Image resize(Image image, int width, int height)
    {
        image.Mutate(x => x.Resize(width, height));
        return image;
    }

    /// <summary>
    /// Rotates the image by the given angle
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="angle">The angle to rotate the image by</param>
    /// <returns>The image</returns>
    public Image rotate(Image image, float angle)
    {
        image.Mutate(x => x.Rotate(angle));
        return image;
    }

    /// <summary>
    /// Saturates the image by the given amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image saturate(Image image, float amount)
    {
        image.Mutate(x => x.Saturate(amount));
        return image;
    }

    /// <summary>
    /// Applies sepia toning to the image by the given amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="amount">The amount</param>
    /// <returns>The image</returns>
    public Image sepia(Image image, float amount)
    {
        image.Mutate(x => x.Sepia(amount));
        return image;
    }

    /// <summary>
    /// Skews the image by the given amount
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="degreesX">The angle in degrees to perform the skew along the x-axis.</param>
    /// <param name="degreesY">The angle in degrees to perform the skew along the y-axis.</param>
    /// <returns>The image</returns>
    public Image skew(Image image, float degreesX, float degreesY)
    {
        image.Mutate(x => x.Skew(degreesX, degreesY));
        return image;
    }

    /// <summary>
    /// Applies a radial vignette effect to the given image.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="color">The color to apply</param>
    /// <returns>The image</returns>
    public Image vignette(Image image, string color)
    {
        var c = color.ParseColor();
        image.Mutate(x => x.Vignette(c));
        return image;
    }
}
