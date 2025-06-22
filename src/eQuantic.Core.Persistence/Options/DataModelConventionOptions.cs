namespace eQuantic.Core.Persistence.Options;

public enum NamingCase
{
    PascalCase,
    CamelCase,
    SnakeCase
}

public class DataModelConventionOptions
{
    internal bool? EntityAuditingEnabled { get; private set; }
    
    public DataModelConventionOptions EnableEntityAuditing(bool enabled = true)
    {
        EntityAuditingEnabled = enabled;
        return this;
    }
}