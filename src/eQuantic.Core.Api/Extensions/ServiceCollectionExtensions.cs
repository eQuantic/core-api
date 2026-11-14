using System.Reflection;
using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Modules;
using eQuantic.Core.Api.Options;
using eQuantic.Core.Api.Swagger;
using eQuantic.Linq.Web;
using eQuantic.Linq.Web.Swashbuckle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Extensions;

/// <summary>
/// The Service Collection Extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add the api documentation
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="options">The documentation options</param>
    /// <param name="swaggerGenOptions">The swagger gen options</param>
    /// <returns>The registry</returns>
    public static IServiceCollection AddApiDocumentation(
        this IServiceCollection services,
        Action<DocumentationOptions>? options = null,
        Action<SwaggerGenOptions>? swaggerGenOptions = null)
    {
        var docOptions = new DocumentationOptions();
        options?.Invoke(docOptions);

        services.AddSingleton(docOptions);
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = docOptions.Title, Version = "v1" });
            c.DescribeAllParametersInCamelCase();

            if(!string.IsNullOrEmpty(docOptions.XmlCommentsFile))
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, docOptions.XmlCommentsFile));

            c.EnableAnnotations();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme.<br />
                Enter 'Bearer' [space] and then your token in the text input below.<br />
                Example: ""Bearer 12345abcdef""",
            });
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });

            // eQuantic.Linq v3 query documentation: EntityQuery<T>/EntityQueryModel<T> endpoints
            // and PagedListRequest<TEntity> envelopes (filterBy/orderBy keys).
            var queryOptions = new QueryStringOptions { FilterKey = "filterBy", OrderByKey = "orderBy" };
            c.AddEntityQueryDocumentation(queryOptions);
            c.OperationFilter<PagedListRequestOperationFilter>(queryOptions);

            swaggerGenOptions?.Invoke(c);
        });
        return services;
    }

    /// <summary>
    /// Add exception filter
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddExceptionFilter(this IServiceCollection services,
        Action<ExceptionFilterOptions>? options = null)
    {
        var filterOptions = new ExceptionFilterOptions();
        options?.Invoke(filterOptions);

        services.AddSingleton(filterOptions);
        services.AddTransient<ExceptionMiddleware>();
        return services;
    }

    /// <summary>
    /// Registers every concrete <see cref="IEndpointModule"/> declared in the given assemblies, so that
    /// <see cref="WebApplicationExtensions.MapModules"/> maps their endpoints. With no assembly given, it
    /// looks in the entry assembly. Calling it again for the same assembly registers nothing twice.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblies">The assemblies that declare the modules</param>
    /// <returns>The service collection</returns>
    /// <exception cref="InvalidOperationException">No assembly was given and the process has no entry assembly.</exception>
    public static IServiceCollection AddEndpointModules(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        if (assemblies.Length == 0)
        {
            var entry = Assembly.GetEntryAssembly()
                ?? throw new InvalidOperationException(
                    "There is no entry assembly to look for endpoint modules in. Pass the assemblies that declare them.");
            assemblies = [entry];
        }

        foreach (var assembly in assemblies.Distinct())
        {
            ArgumentNullException.ThrowIfNull(assembly, nameof(assemblies));

            foreach (var type in assembly.GetTypes())
            {
                if (type is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false }
                    && typeof(IEndpointModule).IsAssignableFrom(type))
                {
                    services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpointModule), type));
                }
            }
        }

        return services;
    }
}
