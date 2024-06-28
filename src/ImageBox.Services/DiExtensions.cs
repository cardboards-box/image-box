﻿namespace ImageBox.Services;

using Loading;

/// <summary></summary>
public static class DiExtensions
{
    /// <summary></summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IContextGeneratorService, ContextGeneratorService>()
            .AddTransient<IElementReflectionService, ElementReflectionService>()
            .AddTransient<IScriptExecutionService, ScriptExecutionService>()
            .AddTransient<ITemplateLoaderService, TemplateLoaderService>();
    }
}