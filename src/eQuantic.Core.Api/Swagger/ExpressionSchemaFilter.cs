#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Swagger;

/// <summary>
/// The expression schema filter class
/// </summary>
/// <seealso cref="ISchemaFilter"/>
public class ExpressionSchemaFilter<TColumn> : ISchemaFilter
{
    
#if NET10_0_OR_GREATER
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if(context.Type != typeof(TColumn))
            return;
        
        schema = new OpenApiSchema
        {
            Type = JsonSchemaType.String,
        };
    }
#else
    /// <summary>
    /// Applies the schema
    /// </summary>
    /// <param name="schema">The schema</param>
    /// <param name="context">The context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if(context.Type != typeof(TColumn))
            return;

        schema.Type = "string";
    }
#endif
}