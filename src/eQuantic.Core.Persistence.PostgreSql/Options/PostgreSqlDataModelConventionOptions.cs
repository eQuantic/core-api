using eQuantic.Core.Persistence.Relational.Options;

namespace eQuantic.Core.Persistence.PostgreSql.Options;

public class PostgreSqlDataModelConventionOptions : RelationalDataModelConventionOptions
{
    public PostgreSqlDataModelConventionOptions()
    {
    }

    public PostgreSqlDataModelConventionOptions(RelationalDataModelConventionOptions options) : base(options)
    {
    }
}