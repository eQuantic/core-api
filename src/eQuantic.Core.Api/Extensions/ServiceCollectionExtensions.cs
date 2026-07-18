using eQuantic.Core.Api.Middlewares;
using eQuantic.Core.Api.Options;
using eQuantic.Core.Api.Swagger;
using eQuantic.Linq.Web;
using eQuantic.Linq.Web.Swashbuckle;
using Microsoft.Extensions.DependencyInjection;
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
}
