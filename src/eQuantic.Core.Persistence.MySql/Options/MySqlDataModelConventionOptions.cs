using eQuantic.Core.Persistence.Relational.Options;

namespace eQuantic.Core.Persistence.MySql.Options;

public class MySqlDataModelConventionOptions : RelationalDataModelConventionOptions
{
    public MySqlDataModelConventionOptions()
    {
    }

    public MySqlDataModelConventionOptions(RelationalDataModelConventionOptions options) : base(options)
    {
    }
}