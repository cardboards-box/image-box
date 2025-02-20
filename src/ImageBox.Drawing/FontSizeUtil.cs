using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ImageBox.Drawing;

/// <summary>
/// Utility class for font sizes
/// </summary>
public static class FontSizeUtil
{
    /// <summary>
    /// Determines the font size for the given text
    /// </summary>
    /// <param name="text">The text to determine the font size for</param>
    /// <param name="rect">The bounds to fit the text in</param>
    /// <param name="padding">The padding to use for the rectangle</param>
    /// <param name="options">The options for the text renderer</param>
    /// <returns>The size of the font</returns>
    public static float DetermineFontSize(string text, Rectangle rect, float padding, TextOptions options)
    {
        var targetWidth = rect.Width - (padding * 2);
        var targetHeight = rect.Height - (padding * 2);

        float minFontSize = 1;
        var maxFontSize = targetHeight;

        var currentBounds = TextMeasurer.MeasureAdvance(text, options);
        if (currentBounds.Width < targetWidth)
            maxFontSize = MathF.Floor(maxFontSize * (targetWidth / currentBounds.Width));

        while (minFontSize < maxFontSize)
        {
            var midFontSize = (minFontSize + maxFontSize) / 2;
            Font midFont = new(options.Font, midFontSize);
            currentBounds = TextMeasurer.MeasureAdvance(text, new TextOptions(midFont)
            {
                WrappingLength = targetWidth
            });

            if (currentBounds.Height > targetHeight)
                maxFontSize = midFontSize - 0.1f;
            else
                minFontSize = midFontSize + 0.1f;
        }

        return minFontSize;
    }

    /// <summary>
    /// Determines the font size for the given text
    /// </summary>
    /// <param name="text">The text to determine the font size for</param>
    /// <param name="rect">The bounds to fit the text in</param>
    /// <param name="padding">The padding to use for the rectangle</param>
    /// <param name="font">The font to use</param>
    /// <param name="workBreaking">How to word breaking works when text wrapping</param>
    /// <returns>The size of the font</returns>
    public static float DetermineFontSize(string text, Rectangle rect, float padding, Font font, WordBreaking workBreaking)
    {
        return DetermineFontSize(text, rect, padding, new TextOptions(font)
        {
            WordBreaking = workBreaking
        });
    }
}
