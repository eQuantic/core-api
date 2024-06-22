using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Modules;
using eQuantic.Core.Api.Options;
using eQuantic.Core.Api.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Api.Extensions;

/// <summary>
/// The wep application extensions class
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Use the api documentation
    /// </summary>
    /// <param name="app">The app</param>
    /// <param name="options">The documentation options</param>
    /// <returns>The app</returns>
    public static WebApplication UseApiDocumentation(this WebApplication app, Action<DocumentationOptions>? options = null)
    {
        var docOptions = app.Services.GetService<DocumentationOptions>() ?? new DocumentationOptions();
        options?.Invoke(docOptions);

        var hasSignIn = !string.IsNullOrEmpty(docOptions.SignInUrl);
        
        if(hasSignIn)
            app.MapGet("/swagger/swagger.js", () =>
            {
                var swaggerJson = new SwaggerJson(docOptions);
                return Results.Text(swaggerJson.TransformText(), contentType: "application/json");
            });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", docOptions.Title);
            
            if(hasSignIn)
                c.InjectJavascript("/swagger/swagger.js");
        });
        return app;
    }

    /// <summary>
    /// Use the exception filter
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseExceptionFilter(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
    
    /// <summary>
    /// Map modules
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication MapModules(this WebApplication app)
    {
        var modules = app.Services.GetServices<IEndpointModule>();
        foreach (var module in modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}