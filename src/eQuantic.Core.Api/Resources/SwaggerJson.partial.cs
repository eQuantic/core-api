using eQuantic.Core.Api.Options;

namespace eQuantic.Core.Api.Resources;

public partial class SwaggerJson(DocumentationOptions options)
{
    public DocumentationOptions DocumentationOptions { get; } = options;
}