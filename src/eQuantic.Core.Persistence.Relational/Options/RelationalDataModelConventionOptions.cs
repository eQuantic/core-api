using eQuantic.Core.Persistence.Options;

namespace eQuantic.Core.Persistence.Relational.Options;

public class RelationalDataModelConventionOptions : DataModelConventionOptions
{
    internal NamingCase? NamingCase { get; private set; }
    internal bool? FullyQualifiedPrimaryKeysEnabled { get; private set; }
    internal string? Suffix { get; private set; }

    public RelationalDataModelConventionOptions()
    {
    }

    public RelationalDataModelConventionOptions(DataModelConventionOptions options)
    {
        if(options.EntityAuditingEnabled.HasValue)
            EnableEntityAuditing(options.EntityAuditingEnabled.Value);
    }
    public RelationalDataModelConventionOptions(RelationalDataModelConventionOptions options)
    {
        if(options.EntityAuditingEnabled.HasValue)
            EnableEntityAuditing(options.EntityAuditingEnabled.Value);
        
        NamingCase = options.NamingCase;
        FullyQualifiedPrimaryKeysEnabled = options.FullyQualifiedPrimaryKeysEnabled;
        Suffix = options.Suffix;
    }

    public RelationalDataModelConventionOptions UseFullyQualifiedPrimaryKeys(bool enabled = true)
    {
        FullyQualifiedPrimaryKeysEnabled = enabled;
        return this;
    }
    
    public RelationalDataModelConventionOptions UseSnakeCase()
    {
        NamingCase = eQuantic.Core.Persistence.Options.NamingCase.SnakeCase;
        return this;
    }
    
    public RelationalDataModelConventionOptions UsePascalCase()
    {
        NamingCase = eQuantic.Core.Persistence.Options.NamingCase.PascalCase;
        return this;
    }
    
    public RelationalDataModelConventionOptions UseCamelCase()
    {
        NamingCase = eQuantic.Core.Persistence.Options.NamingCase.CamelCase;
        return this;
    }
    
    public RelationalDataModelConventionOptions RemoveEntitySuffixFromTableName(string suffix = "Data")
    {
        Suffix = suffix;
        return this;
    }
}