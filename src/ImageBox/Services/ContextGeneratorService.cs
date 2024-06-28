using Module = Esprima.Ast.Module;
using Jint;

namespace ImageBox.Services;

using Elements.Elements.Other;
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
    Task<RenderContext> Generate(BoxedImage image);
}

internal class ContextGeneratorService(
    IFileResolverService _resolver,
    IBoxedImageConfig _config,
    ILogger<ContextGeneratorService> _logger) : IContextGeneratorService
{
    /// <summary>
    /// Generates the render context from the given boxed image
    /// </summary>
    /// <param name="image">The boxed image to create the context for</param>
    /// <returns>The render context</returns>
    public async Task<RenderContext> Generate(BoxedImage image)
    {
        //Get the template element from the boxed image
        var template = GetTemplate(image);
        //Get the script runner from the script elements
        var runner = await GetRunner(image);
        //Get the size context from the template element's attributes
        var context = GetContext(template, image);
        //Get all of the font families from the image
        var fontFamilies = image.Elements.OfType<FontFamilyElem>().ToArray();
        //Return the created render context
        return new RenderContext
        {
            Image = image,
            Template = template,
            Runner = runner,
            Size = context,
            FontFamilies = fontFamilies
        };
    }

    /// <summary>
    /// Determines the size of the image from the context
    /// </summary>
    /// <param name="template">The template to get the context from</param>
    /// <param name="image">The image the template is from</param>
    /// <returns>The size context of the image</returns>
    /// <exception cref="RenderContextException">Thrown if a required property is missing</exception>
    public SizeContext GetContext(TemplateElem template, BoxedImage image)
    {
        //Validate width and height
        if (template.Width is null)
            throw new RenderContextException("Template width could not be determined", image, template.Context);
        if (template.Height is null)
            throw new RenderContextException("Template height could not be determined", image, template.Context);
        //Get the font size, width, and height
        var fontSize = (template.FontSize ?? _config.FontSize).Pixels();
        var width = template.Width.Value.Pixels(null, true);
        var height = template.Height.Value.Pixels(null, false);
        //Generate size context from sizing units
        return SizeContext.ForRoot(width, height, fontSize);
    }

    /// <summary>
    /// Gets the script runner for the boxed image
    /// </summary>
    /// <param name="image">The boxed image</param>
    /// <returns>The script runner</returns>
    /// <exception cref="RenderContextException">Thrown if any exception occurs during preparation</exception>
    public async Task<ScriptRunner?> GetRunner(BoxedImage image)
    {
        try
        {
            //Get all of the scripts for the context
            var scripts = GetScripts(image, out var setupScript);
            //No setup script? don't bother processing
            if (setupScript is null) return null;
            //Setup the script runner
            var runner = new ScriptRunner(
                _config.ScriptTimeout.Milliseconds * 1000,
                _config.ScriptRecursionLimit,
                _config.ScriptMemoryLimitMb);
            //Add the standard context to the runner
            //This adds the `system` module with drawing and context classes
            AddStandardContext(runner);
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
    public async IAsyncEnumerable<RenderModule> Modules(ScriptElem[] scripts, BoxedImage image)
    {
        //Iterate over the scripts and resolve and prepare them
        foreach(var script in scripts)
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
    public async Task<Prepared<Module>> GetScript(ScriptElem script, BoxedImage image)
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
    /// <param name="setup">The setup script (if one is included)</param>
    /// <returns>All of the script elements</returns>
    /// <exception cref="RenderContextException">Thrown if multiple setup scripts listed in the template</exception>
    /// <exception cref="RenderContextException">Thrown if a script module name is not set</exception>
    /// <exception cref="RenderContextException">Thrown if no setup script is listed in the template and there are other scripts</exception>
    public static ScriptElem[] GetScripts(BoxedImage image, out ScriptElem? setup)
    {
        setup = null;
        var scripts = new List<ScriptElem>();
        //Iterate through all of the script elements in the root context of the template
        foreach(var script in image.Elements.OfType<ScriptElem>())
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
        return [..scripts];
    }

    /// <summary>
    /// Gets the template element from the given image
    /// </summary>
    /// <param name="image">The image to get the template for</param>
    /// <returns>The template element</returns>
    /// <exception cref="RenderContextException">Thrown if there are no template elements</exception>
    /// <exception cref="RenderContextException">Thrown if there are more than one template elements</exception>
    public static TemplateElem GetTemplate(BoxedImage image)
    {
        //Get all of the template elements from the image
        var template = image.Elements
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
        return template.First();
    }

    /// <summary>
    /// Adds the standard system modules to the given script runner
    /// </summary>
    /// <param name="runner">The script runner to add to</param>
    public static void AddStandardContext(ScriptRunner runner)
    {
        runner.AddModule("system", t => t
            .ExportType<Drawing>()
            .ExportType<Context>());
    }

    /// <summary>
    /// Represents a script module that is loaded into the script runner
    /// </summary>
    /// <param name="Name">The module name of the script</param>
    /// <param name="Module">The prepared script</param>
    public record class RenderModule(string Name, Prepared<Module> Module);
}