using eQuantic.Core.Persistence.Relational.Options;

namespace eQuantic.Core.Persistence.SqlServer.Options;

public class SqlServerDataModelConventionOptions : RelationalDataModelConventionOptions
{
    public SqlServerDataModelConventionOptions()
    {
    }

    public SqlServerDataModelConventionOptions(RelationalDataModelConventionOptions options) : base(options)
    {
    }
}