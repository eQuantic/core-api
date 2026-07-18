using eQuantic.Core.Domain.Entities.Requests;
using eQuantic.Linq.Web;
using eQuantic.Linq.Web.Documentation;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eQuantic.Core.Api.Swagger;

/// <summary>
/// Documents endpoints that receive <see cref="PagedListRequest{TEntity}"/>: the
/// <c>filterBy</c>/<c>orderBy</c> parameters get the complete eQuantic.Linq v3 syntax, the
/// entity's actual member paths (camelCase, navigations, collections, enum values,
/// <c>[Column]</c> aliases) and examples generated from the entity's members.
/// </summary>
public class PagedListRequestOperationFilter : IOperationFilter
{
    private readonly QueryStringOptions _options;

    /// <summary>Creates the filter.</summary>
    /// <param name="options">Query-string options; key names are honored. Defaults apply when omitted.</param>
    public PagedListRequestOperationFilter(QueryStringOptions? options = null) =>
        _options = options ?? new QueryStringOptions();

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is not { Count: > 0 })
        {
            return;
        }

        foreach (var parameter in context.MethodInfo.GetParameters())
        {
            if (!TryGetEntityType(parameter.ParameterType, out var entityType))
            {
                continue;
            }

            var documentation = EntityQueryDocumentation.For(entityType, _options);
            foreach (var doc in documentation.Parameters)
            {
                var operationParameter = operation.Parameters.FirstOrDefault(p =>
                    string.Equals(p.Name, doc.Name, StringComparison.OrdinalIgnoreCase));
                if (operationParameter is OpenApiParameter concrete &&
                    doc.Kind is EntityQueryParameterKind.Filter or EntityQueryParameterKind.OrderBy)
                {
                    concrete.Description = doc.Description;
                    concrete.Example = doc.Example is null
                        ? null
                        : System.Text.Json.Nodes.JsonValue.Create(doc.Example);
                }
            }
        }
    }

    private static bool TryGetEntityType(Type parameterType, out Type entityType)
    {
        entityType = typeof(object);
        for (var type = parameterType; type is not null && type != typeof(object); type = type.BaseType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PagedListRequest<>))
            {
                entityType = type.GetGenericArguments()[0];
                return true;
            }
        }

        return false;
    }
}
