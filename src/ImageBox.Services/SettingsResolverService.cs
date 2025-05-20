using Jint;
using Jint.Native.Object;

namespace ImageBox.Services;

using Ast;
using Elements.TopLevel;

/// <summary>
/// Service for resolving settings for the templates
/// </summary>
public interface ISettingsResolverService
{
    /// <summary>
    /// Gets the template settings from the given template and image
    /// </summary>
    /// <param name="template">The template element</param>
    /// <param name="image">The template AST</param>
    /// <param name="init">The initialization script</param>
    /// <returns>The template settings</returns>
    Task<TemplateSettings> GetSettings(TemplateElem template, LoadedAst image, ScriptRunner? init);
}

internal class SettingsResolverService(IServiceConfig _config) : ISettingsResolverService
{
    /// <summary>
    /// Validates the value of a property
    /// </summary>
    /// <param name="value">The value being validated</param>
    /// <param name="property">The name of the property being validated</param>
    /// <param name="element">The element being validated</param>
    /// <param name="image">The image being validated</param>
    /// <param name="max">The maximum value allowed for the property</param>
    /// <param name="min">The minimum value allowed for the property</param>
    /// <exception cref="RenderContextException">Thrown if the property isn't valid</exception>
    public static void Validate(double value, string property, AstElement? element, LoadedAst image, double max, double min = 1)
    {
        if (value < min)
            throw new RenderContextException($"Invalid {property} - Current Value: {value}. Must be greater than {min}", image, element);

        if (value > max)
            throw new RenderContextException($"Invalid {property} - Current Value: {value}. Must be less than {max}", image, element);
    }

    /// <summary>
    /// Validates the width and height of the image
    /// </summary>
    /// <param name="width">The width of the image in pixels</param>
    /// <param name="height">The height of the image in pixels</param>
    /// <param name="image">The image being validated</param>
    /// <param name="element">The element being validated</param>
    public void Validate(int width, int height, LoadedAst image, AstElement? element)
    {
        if (_config.Render.MaxHeightUnit is not null)
            Validate(height, nameof(height), element, image, _config.Render.MaxHeightUnit.Value.Pixels(null, false));

        if (_config.Render.MaxWidthUnit is not null)
            Validate(width, nameof(width), element, image, _config.Render.MaxWidthUnit.Value.Pixels(null, true));
    }

    /// <summary>
    /// Determines the default size of the image from the context
    /// </summary>
    /// <param name="template">The template to get the context from</param>
    /// <param name="image">The image the template is from</param>
    /// <returns>The size context of the image</returns>
    /// <exception cref="RenderContextException">Thrown if a required property is missing</exception>
    /// <exception cref="RenderContextException">Thrown if a property is invalid</exception>
    public SizeContext GetDefaultContext(TemplateElem template, LoadedAst image)
    {
        var widthUnit = template.Width ?? _config.Render.WidthUnit;
        var heightUnit = template.Height ?? _config.Render.HeightUnit;
        //Validate width and height
        if (widthUnit.Value <= 0)
            throw new RenderContextException("Template width could not be determined", image, template.Context);
        if (heightUnit.Value <= 0)
            throw new RenderContextException("Template height could not be determined", image, template.Context);
        //Get the font size, width, and height
        var fontSize = (template.FontSize ?? _config.Render.FontSizeUnit).Pixels();
        var width = widthUnit.Pixels(null, true);
        var height = heightUnit.Pixels(null, false);
        var fontFamily = template.FontFamily ?? _config.Render.FontFamily ?? string.Empty;
        //Validate the width and height
        Validate(width, height, image, template.Context);
        //Generate size context from sizing units
        return SizeContext.ForRoot(width, height, fontSize, fontFamily);
    }

    /// <summary>
    /// Gets the settings value from the given script runner
    /// </summary>
    /// <param name="ctx">The default context to use</param>
    /// <param name="init">The script runner for the init function</param>
    /// <returns>The fetched JSValue</returns>
    public static async Task<ObjectInstance?> GetSettingsFromInit(SizeContext ctx, ScriptRunner? init)
    {
        //No init? skip it
        if (init is null) return null;
        //Get the JSValue representing the result
        var result = await init.Execute(ctx);
        //If the result is null or not an object, skip it
        return result is null || !result.IsObject() 
            ? null
            : result.AsObject();
    }

    /// <summary>
    /// Fill in the default size context with the given init function result
    /// </summary>
    /// <param name="defCtx">The default context to use</param>
    /// <param name="init">The init function result</param>
    /// <param name="image">The image the template is from</param>
    /// <param name="template">The template to get the context from</param>
    /// <returns>The size context to use</returns>
    public SizeContext SizeFromInit(SizeContext defCtx, ObjectInstance? init, LoadedAst image, TemplateElem template)
    {
        bool TryGetSize(string name, bool isWidth, [MaybeNullWhen(false)] out int output)
        {
            return init.TryGetSize(name, defCtx, isWidth, out output);
        }

        if (init is null) return defCtx;

        if (!TryGetSize("width", true, out var width))
            width = defCtx.Width;

        if (!TryGetSize("height", false, out var height))
            height = defCtx.Height;

        if (!TryGetSize("fontSize", true, out var fontSize) &&
            !TryGetSize("font-size", true, out fontSize))
            fontSize = defCtx.FontSize;

        if (!init.TryGetString("fontFamily", out var fontFamily) &&
            !init.TryGetString("font-family", out fontFamily))
            fontFamily = defCtx.FontFamily;

        //Validate the width and height
        Validate(width, height, image, template.Context);
        //Generate size context from sizing units
        return SizeContext.ForRoot(width, height, fontSize, fontFamily?.ForceNull() ?? string.Empty);
    }

    /// <inheritdoc/>
    public async Task<TemplateSettings> GetSettings(TemplateElem template, LoadedAst image, ScriptRunner? init)
    {
        bool DetermineAnimate(ObjectInstance? settings)
        {
            if (settings.TryGetString("animate", out var animateTest))
                return animateTest.EqualsIc("true");

            return template.Animate;
        }

        TimeUnit GetAnimationDuration(ObjectInstance? settings)
        {
            string[] keys =
            [
                "animation-duration",
                "animate-duration",
                "animationDuration",
                "animateDuration",
                "duration"
            ];
            foreach (var key in keys)
            {
                if (settings.TryGetTime(key, out var output))
                    return output;
            }

            return template.AnimateDuration ?? _config.Render.AnimateDurationUnit;
        }

        double GetAnimationFps(ObjectInstance? settings)
        {
            string[] keys =
            [
                "animation-fps",
                "animate-fps",
                "animationFps",
                "animateFps",
                "fps"
            ];
            foreach (var key in keys)
            {
                if (settings.TryGetDouble(key, out var output))
                    return output;
            }

            return template.AnimateFps ?? _config.Render.AnimateFps;
        }

        ushort GetAnimationRepeat(ObjectInstance? settings)
        {
            string[] keys =
            [
                "animation-repeat",
                "animate-repeat",
                "animationRepeat",
                "animateRepeat",
                "repeat"
            ];
            foreach (var key in keys)
            {
                if (settings.TryGetUshort(key, out var output))
                    return output;
            }

            return template.AnimateRepeat ?? _config.Render.AnimateRepeat;
        }

        //Get the default size context from the settings or template
        var defSize = GetDefaultContext(template, image);
        //Get the settings from the init function
        var settings = await GetSettingsFromInit(defSize, init);
        //Get the final size to use for the template
        var size = SizeFromInit(defSize, settings, image, template);
        //Get the current template settings
        var output = new TemplateSettings { Size = size };
        //If "animate" is specified and not true, skip the animation
        if (!DetermineAnimate(settings))
            return output;
        
        //Get the animation duration from wherever we can
        var duration = GetAnimationDuration(settings);
        var durationSec = duration.Milliseconds / 1000;
        if (durationSec < 1) return output;

        //Get the animation frames per second from wherever we can
        var fps = GetAnimationFps(settings);
        if (fps < 1) return output;

        var totalFrames = (uint)Math.Round(durationSec * fps, 0);
        var frameDelay = (uint)Math.Round(duration.Milliseconds / totalFrames, 0);
        var frameRepeat = GetAnimationRepeat(settings);

        //Validate the total number of frames that can be rendered
        if (_config.Render.MaxTotalFrames is not null && _config.Render.MaxTotalFrames > 0)
            Validate(totalFrames, nameof(totalFrames), template.Context, image, _config.Render.MaxTotalFrames.Value);

        output.FrameDelay = frameDelay;
        output.FrameRepeat = frameRepeat;
        output.TotalFrames = totalFrames;
        return output;
    }
}
