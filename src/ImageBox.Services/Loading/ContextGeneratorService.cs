using Module = Acornima.Ast.Module;
using Jint;

namespace ImageBox.Services.Loading;

using Scripting;
using SystemModules;

/// <summary>
/// A service for generating render contexts and resolving scripts
/// </summary>
public interface IContextGeneratorService
{
    /// <summary>
    /// Generates the render context from the given boxed image
    /// </summary>
    /// <param name="image">The boxed image to create the context for</param>
    /// <returns>The render context</returns>
    /// <exception cref="RenderContextException">Thrown if a required property is missing</exception>
    /// <exception cref="RenderContextException">Thrown if any exception occurs during preparation</exception>
    /// <exception cref="RenderContextException">Thrown if a remote script failed to resolve</exception>
    /// <exception cref="RenderContextException">Thrown if the script body is empty</exception>
    /// <exception cref="RenderContextException">Thrown if multiple setup scripts listed in the template</exception>
    /// <exception cref="RenderContextException">Thrown if a script module name is not set</exception>
    /// <exception cref="RenderContextException">Thrown if no setup script is listed in the template and there are other scripts</exception>
    /// <exception cref="RenderContextException">Thrown if there are no template elements</exception>
    /// <exception cref="RenderContextException">Thrown if there are more than one template elements</exception>
    Task<ContextBox> Generate(LoadedAst image);
}

internal class ContextGeneratorService(
    IServiceConfig _config,
    IFileResolverService _resolver,
    IElementReflectionService _elements,
    ILogger<ContextGeneratorService> _logger) : IContextGeneratorService
{
    /// <summary>
    /// Generates the render context from the given boxed image
    /// </summary>
    /// <param name="image">The boxed image to create the context for</param>
    /// <returns>The render context</returns>
    public async Task<ContextBox> Generate(LoadedAst image)
    {
        //Get all of the elements from box
        var elements = _elements.BindTemplates(image.SyntaxTree, false).ToArray();
        //Get the template element from the boxed image
        var template = GetTemplate(image, elements);
        //Get the script runner from the script elements
        var runner = await GetRunner(image, elements);
        //Get the size context from the template element's attributes
        var context = GetContext(template, image);
        //Get all of the font families from the image
        var (fonts, cache) = await GetResources(image, elements);
        //Determine animation stuff
        uint totalFrames = 1, frameDelay = 0;
        ushort frameRepeat = _config.Render.AnimateRepeat;
        if (template.Animate)
        {
            if (template.AnimateDuration is null)
                throw new RenderContextException("Template animation is enabled but animation duration is not set", image, template.Context);
            //Total number of seconds for the animation
            var duration = template.AnimateDuration.Value.Milliseconds / 1000;
            //Total number of frames
            var fps = template.AnimateFps ?? _config.Render.AnimateFps;
            totalFrames = (uint)Math.Round(duration * fps, 0);
            frameDelay = (uint)(template.AnimateDuration.Value.Milliseconds / totalFrames);
            frameRepeat = template.AnimateRepeat ?? _config.Render.AnimateRepeat;
        }

        return new ContextBox 
        {
            Ast = image,
            Template = template.Context!,
            Fonts = fonts,
            Size = context,
            Runner = runner,
            TotalFrames = totalFrames,
            FrameDelay = frameDelay,
            FrameRepeat = frameRepeat,
        };
    }

    /// <summary>
    /// Gets all of the fonts from the element templates
    /// </summary>
    /// <param name="families">The font families to load</param>
    /// <param name="wrkDir">The working directory</param>
    /// <returns>The loaded font families</returns>
    public async Task<ContextFonts> GetFonts(IEnumerable<FontFamilyElem> families, string wrkDir)
    {
        var fonts = new ContextFonts();

        foreach(var family in families)
        {
            if (family.Source is null) continue;

            var path = family.Source.Value.GetAbsolute(wrkDir);
            var (stream, _, _) = await _resolver.Fetch(path);
            var ff = fonts.Collection.Add(stream);

            var loaded = new LoadedFont
            { 
                Element = family,
                Family = ff,
            };
            
            if (!fonts.Families.TryAdd(loaded.Name, loaded))
                throw new RenderContextException($"Font family with the name '{loaded.Name}' has already been loaded", family.Context);
        }

        return fonts;
    }

    /// <summary>
    /// Determines the size of the image from the context
    /// </summary>
    /// <param name="template">The template to get the context from</param>
    /// <param name="image">The image the template is from</param>
    /// <returns>The size context of the image</returns>
    /// <exception cref="RenderContextException">Thrown if a required property is missing</exception>
    public SizeContext GetContext(TemplateElem template, LoadedAst image)
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
        //Generate size context from sizing units
        return SizeContext.ForRoot(width, height, fontSize, fontFamily);
    }

    /// <summary>
    /// Gets the script runner for the boxed image
    /// </summary>
    /// <param name="image">The boxed image</param>
    /// <param name="elements">The elements to get the script for</param>
    /// <returns>The script runner</returns>
    /// <exception cref="RenderContextException">Thrown if any exception occurs during preparation</exception>
    public async Task<ScriptRunner?> GetRunner(LoadedAst image, IElement[] elements)
    {
        try
        {
            //Get all of the scripts for the context
            var scripts = GetScripts(image, elements, out var setupScript);
            //No setup script? don't bother processing
            if (setupScript is null) return null;
            //Setup the script runner
            var runner = new ScriptRunner(
                _config.Scripts.TimeoutUnit.Milliseconds * 1000,
                _config.Scripts.RecursionLimit,
                _config.Scripts.MemoryLimitMb);
            //Add the standard context to the runner
            //This adds the `system` module with drawing and context classes
            AddStandardContext(runner, image);
            //Prepare the setup script
            var setup = await GetScript(setupScript, image);
            //Add the setup script to the runner
            runner.AddModule("face-script", setup);
            //Get all of the compiled scripts
            var compiledScripts = Modules(scripts, image);
            //Add the compiled scripts to the runner
            await foreach (var script in compiledScripts)
                runner.AddModule(script.Name, script.Module);
            //Set the main script to the runner
            runner.SetScript(@"
import FaceScript from 'face-script';
export function main(args) { 
    return FaceScript(args); 
}");
            //return the prepared context
            return runner;
        }
        catch (RenderContextException)
        {
            //Don't re-box the exception
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get script runner: {name} ({dir})", image.FileName, image.WorkingDirectory);
            throw new RenderContextException("Failed to get script runner", image, ex);
        }
    }

    /// <summary>
    /// Gets all of the prepared scripts from the given script collection
    /// </summary>
    /// <param name="scripts">The script elements to prepare</param>
    /// <param name="image">The image that is loading the elements</param>
    /// <returns>All of the prepared script modules</returns>
    public async IAsyncEnumerable<RenderModule> Modules(ScriptElem[] scripts, LoadedAst image)
    {
        //Iterate over the scripts and resolve and prepare them
        foreach (var script in scripts)
        {
            //Resolve the scripts
            var module = await GetScript(script, image);
            //Return the prepared module
            yield return new RenderModule(script.Module!, module);
        }
    }

    /// <summary>
    /// Resolves the script from the script element
    /// </summary>
    /// <param name="script">The script element to resolve</param>
    /// <param name="image">The image that is loading the script</param>
    /// <returns>The prepared script</returns>
    /// <exception cref="RenderContextException">Thrown if a remote script failed to resolve</exception>
    /// <exception cref="RenderContextException">Thrown if the script body is empty</exception>
    public async Task<Prepared<Module>> GetScript(ScriptElem script, LoadedAst image)
    {
        //Get the script body from the element
        var value = script.Value;
        if (script.Source is not null)
        {
            //Get the absolute path of the script
            var actualPath = script.Source.Value.GetAbsolute(image.WorkingDirectory);
            try
            {
                //Resolve the script from the source
                var (stream, _, _) = await _resolver.Fetch(actualPath);
                using var reader = new StreamReader(stream);
                value = await reader.ReadToEndAsync();
                await stream.DisposeAsync();
            }
            catch (RenderContextException)
            {
                //Don't re-box the exception
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred while fetching script from source: {Path}. {Context}",
                    actualPath, script.Context?.ExceptionString());
                //Script failed to resolve; box the exception and report it
                throw new RenderContextException(
                    $"Failed to fetch script from source: {actualPath.OSSafe}",
                    ex,
                    image,
                    script.Context);
            }
        }
        //Ensure there is a script body
        if (string.IsNullOrWhiteSpace(value))
            throw new RenderContextException(
                "Script body is empty (or script body resolved from remote source)",
                image,
                script.Context);
        //Prepare the script and return it
        return ScriptRunner.Prepare(value);
    }

    /// <summary>
    /// Gets all of the scripts from the template
    /// </summary>
    /// <param name="image">The image to get the template from</param>
    /// <param name="elements">The elements to get the scripts from</param>
    /// <param name="setup">The setup script (if one is included)</param>
    /// <returns>All of the script elements</returns>
    /// <exception cref="RenderContextException">Thrown if multiple setup scripts listed in the template</exception>
    /// <exception cref="RenderContextException">Thrown if a script module name is not set</exception>
    /// <exception cref="RenderContextException">Thrown if no setup script is listed in the template and there are other scripts</exception>
    public static ScriptElem[] GetScripts(LoadedAst image, IElement[] elements, out ScriptElem? setup)
    {
        setup = null;
        var scripts = new List<ScriptElem>();
        //Iterate through all of the script elements in the root context of the template
        foreach (var script in elements.OfType<ScriptElem>())
        {
            //If the script is setup, treat it differently
            if (script.Setup)
            {
                //THERE CAN ONLY BE ONE!!
                if (setup is not null)
                    throw new RenderContextException(
                        "Multiple setup scripts found",
                        image,
                        setup.Context,
                        script.Context);
                //Set the setup script
                setup = script;
                continue;
            }
            //Ensure the script module name is set
            if (string.IsNullOrEmpty(script.Module))
                throw new RenderContextException(
                    "Script module name not set for non-setup script",
                    image,
                    script.Context);
            //Add the script to the output
            scripts.Add(script);
        }
        //Ensure there is a setup script if there are other scripts
        if (setup is null && scripts.Count > 0)
            throw new RenderContextException(
                "Module scripts included in template but no setup script listed",
                image,
                scripts.Select(t => t.Context).ToArray());
        //Return the non-setup scripts
        return [.. scripts];
    }

    /// <summary>
    /// Gets the template element from the given image
    /// </summary>
    /// <param name="image">The image to get the template for</param>
    /// <param name="elements">The element to render for</param>
    /// <returns>The template element</returns>
    /// <exception cref="RenderContextException">Thrown if there are no template elements</exception>
    /// <exception cref="RenderContextException">Thrown if there are more than one template elements</exception>
    public static TemplateElem GetTemplate(LoadedAst image, IElement[] elements)
    {
        //Get all of the template elements from the image
        var template = elements
            .OfType<TemplateElem>()
            .ToArray();
        //Ensure there is at least one template element
        if (template.Length == 0)
            throw new RenderContextException("No template element found", image);
        //Ensure there is only one template element
        if (template.Length > 1)
            throw new RenderContextException("Multiple template elements found",
                image,
                template.Select(t => t.Context).ToArray());
        //Get the template element
        var temp = template.First();
        if (temp is null || temp.Context is null)
            throw new RenderContextException("Template element is null", image);

        return temp;
    }

    public async Task<(ContextFonts, string)> GetResources(LoadedAst image, IElement[] elements)
    {
        var resources = elements
            .OfType<ResourcesElem>()
            .ToArray();

        var fonts = resources.SelectMany(t => t.Children).OfType<FontFamilyElem>();
        var context = await GetFonts(fonts, image.WorkingDirectory);

        return (context, string.Empty);
    }

    /// <summary>
    /// Adds the standard system modules to the given script runner
    /// </summary>
    /// <param name="runner">The script runner to add to</param>
    /// <param name="ast">The loaded image</param>
    public void AddStandardContext(ScriptRunner runner, LoadedAst ast)
    {
        runner.AddModule("system", t => t
            .ExportType<Drawing>()
            .ExportType<Context>()
            .ExportObject("logger", new Logger(_logger, ast)));
    }

    /// <summary>
    /// Represents a script module that is loaded into the script runner
    /// </summary>
    /// <param name="Name">The module name of the script</param>
    /// <param name="Module">The prepared script</param>
    public record class RenderModule(string Name, Prepared<Module> Module);
}