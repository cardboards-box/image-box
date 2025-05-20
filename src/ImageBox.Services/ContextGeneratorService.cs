using Module = Acornima.Ast.Module;
using Jint;

namespace ImageBox.Services;

using Elements.TopLevel;

internal class ContextGeneratorService(
    IServiceConfig _config,
    IFileResolverService _resolver,
    IElementReflectionService _elements,
    IEnumerable<IModuleSourceService> _modules,
    ISettingsResolverService _settings,
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
        var elements = _elements.BindTemplates(image.SyntaxTree, false, null).ToArray();
        //Get the template element from the boxed image
        var template = GetTemplate(image, elements);
        //Get the script runner from the script elements
        var (setup, init) = await GetRunner(image, elements);
        //Get all of the font families from the image
        var (fonts, _) = await GetResources(image, elements);
        //Get the template settings
        var settings = await _settings.GetSettings(template, image, init);

        return new ContextBox
        {
            Ast = image,
            TemplateElement = template,
            Fonts = fonts,
            Runner = setup,
            Settings = settings,
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

        foreach (var family in families)
        {
            if (family.Source is null) continue;

            var (stream, _, _, _) = await _resolver.Fetch(family.Source.Value, wrkDir);
            var ff = fonts.Collection.Add(stream);

            var loaded = new LoadedFont
            {
                Element = family,
                Family = ff,
                Name = family.Name ?? ff.Name,
            };

            if (!fonts.Families.TryAdd(loaded.Name, loaded))
                throw new RenderContextException($"Font family with the name '{loaded.Name}' has already been loaded", family.Context);
        }

        return fonts;
    }

    /// <summary>
    /// Gets the script runner for the boxed image
    /// </summary>
    /// <param name="image">The boxed image</param>
    /// <param name="elements">The elements to get the script for</param>
    /// <returns>The script runner</returns>
    /// <exception cref="RenderContextException">Thrown if any exception occurs during preparation</exception>
    public async Task<(ScriptRunner? setup, ScriptRunner? init)> GetRunner(LoadedAst image, IElement[] elements)
    {
        async Task<ScriptRunner?> GetSingleRunner(ScriptElem? script, RenderModule[] modules)
        {
            //If the script doesn't exist, skip it.
            if (script is null) return null;

            //Setup the script runner
            var runner = new ScriptRunner(
                _config.Scripts.TimeoutUnit.Milliseconds * 1000,
                _config.Scripts.RecursionLimit,
                _config.Scripts.MemoryLimitMb);
            //Add the standard context to the runner
            //This adds the `system` module with drawing and context classes
            await AddStandardContext(runner, image);
            //Prepare the main script
            var main = await GetScript(script, image);
            //Add the main script to the runner
            runner.AddModule("main-script", main);
            //Add the compiled scripts to the runner
            foreach (var module in modules)
                runner.AddModule(module.Name, module.Module);
            //Set the main script to the runner
            runner.SetScript(@"
import MainScript from 'main-script';

export async function main(args) { 
    return await MainScript(args); 
}");
            return runner;
        }

        try
        {
            //Get all of the scripts for the context
            var scripts = GetScripts(image, elements, out var setupScript, out var initScript);
            //No setup or init script? don't bother processing
            if (setupScript is null && initScript is null) return (null, null);
            //Get all of the compiled scripts
            var modules = await Modules(scripts, image).ToArrayAsync();
            //Get the compiled setup script
            var setup = await GetSingleRunner(setupScript, modules);
            //Get the compiled init script
            var init = await GetSingleRunner(initScript, modules);
            return (setup, init);
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
            var path = script.Source.Value;
            try
            {
                //Resolve the script from the source
                var (stream, _, _, _) = await _resolver.Fetch(path, image.WorkingDirectory);
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
                    path, script.Context?.ExceptionString());
                //Script failed to resolve; box the exception and report it
                throw new RenderContextException(
                    $"Failed to fetch script from source: {path.OSSafe}",
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
                script?.Context);
        //Prepare the script and return it
        return ScriptRunner.Prepare(value);
    }

    /// <summary>
    /// Gets all of the scripts from the template
    /// </summary>
    /// <param name="image">The image to get the template from</param>
    /// <param name="elements">The elements to get the scripts from</param>
    /// <param name="setup">The setup script (if one is included)</param>
    /// <param name="init">The initialization script (if one is included)</param>
    /// <returns>All of the script elements</returns>
    /// <exception cref="RenderContextException">Thrown if multiple setup scripts listed in the template</exception>
    /// <exception cref="RenderContextException">Thrown if a script module name is not set</exception>
    /// <exception cref="RenderContextException">Thrown if no setup script is listed in the template and there are other scripts</exception>
    public static ScriptElem[] GetScripts(LoadedAst image, IElement[] elements, out ScriptElem? setup, out ScriptElem? init)
    {
        setup = null;
        init = null;
        var scripts = new List<ScriptElem>();
        //Iterate through all of the script elements in the root context of the template
        foreach (var script in elements.OfType<ScriptElem>())
        {
            //If the script is an initialization script, treat it differently
            if (script.Init)
            {
                //THERE CAN ONLY BE ONE!!
                if (init is not null)
                    throw new RenderContextException(
                        "Multiple initialization scripts found",
                        image,
                        init.Context,
                        script.Context);
                //Set the setup script
                init = script;
                continue;

            }

            //If the script is setup, treat it differently as well
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
        //Ensure there is a setup or init script if there are other scripts
        if (setup is null && init is null && scripts.Count > 0)
            throw new RenderContextException(
                "Module scripts included in element but no setup or initialization script listed",
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
            throw new RenderContextException("No element element found", image);
        //Ensure there is only one template element
        if (template.Length > 1)
            throw new RenderContextException("Multiple element elements found",
                image,
                template.Select(t => t.Context).ToArray());
        //Get the template element
        var temp = template.First();
        if (temp is null || temp.Context is null)
            throw new RenderContextException("Template element is null", image);

        return temp;
    }

    /// <summary>
    /// Loads all of the resources from the image
    /// </summary>
    /// <param name="image">The image to load the resources for</param>
    /// <param name="elements">The elements that define the resources to load</param>
    /// <returns>All of the loaded fonts</returns>
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
    public async Task AddStandardContext(ScriptRunner runner, LoadedAst ast)
    {
        foreach(var module in _modules)
        {
            var name = module.Name;
            var builder = await module.Register(ast);
            runner.AddModule(name, builder);
        }
    }

    /// <summary>
    /// Represents a script module that is loaded into the script runner
    /// </summary>
    /// <param name="Name">The module name of the script</param>
    /// <param name="Module">The prepared script</param>
    public record class RenderModule(string Name, Prepared<Module> Module);
}