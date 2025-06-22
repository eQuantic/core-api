using eQuantic.Core.DataModel;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Persistence.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace eQuantic.Core.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyDataModelConventions(
        this ModelBuilder modelBuilder,
        Action<DataModelConventionOptions>? configureOptions = null)
    {
        var options = new DataModelConventionOptions();
        configureOptions?.Invoke(options);

        if(!options.EntityAuditingEnabled.HasValue || !options.EntityAuditingEnabled.Value) return modelBuilder;
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureEntityAuditing(entityType, options);
            
            // IEntityOwned - CreatedById (required for non-generic, nullable for generic)
            ConfigureUserAuditProperty<IEntityOwned>(entityType, nameof(IEntityOwned.CreatedById), isRequired: true);
            ConfigureUserAuditProperty(entityType, typeof(IEntityOwned<>), nameof(IEntityOwned.CreatedById), isRequired: true);
    
            // IEntityTrack - UpdatedById (nullable)
            ConfigureUserAuditProperty<IEntityTrack>(entityType, nameof(IEntityTrack.UpdatedById), isRequired: false);
            ConfigureUserAuditProperty(entityType, typeof(IEntityTrack<>), nameof(IEntityTrack.UpdatedById), isRequired: false);
    
            // IEntityHistory - DeletedById (nullable)
            ConfigureUserAuditProperty<IEntityHistory>(entityType, nameof(IEntityHistory.DeletedById), isRequired: false);
            ConfigureUserAuditProperty(entityType, typeof(IEntityHistory<>), nameof(IEntityHistory.DeletedById), isRequired: false);

        }
        
        return modelBuilder;
    }
    
    private static void ConfigureEntityAuditing(IMutableEntityType entityType, DataModelConventionOptions options)
    {
        var clrType = entityType.ClrType;
        
        // IEntityTimeMark - CreatedAt (required, default value)
        if (typeof(IEntityTimeMark).IsAssignableFrom(clrType))
        {
            var createdAtProperty = entityType.FindProperty(nameof(IEntityTimeMark.CreatedAt));
            if (createdAtProperty != null)
            {
                createdAtProperty.IsNullable = false;
            }
        }
        
        // IEntityTimeTrack - UpdatedAt (nullable, no default value)
        if (typeof(IEntityTimeTrack).IsAssignableFrom(clrType))
        {
            var updatedAtProperty = entityType.FindProperty(nameof(IEntityTimeTrack.UpdatedAt));
            if (updatedAtProperty != null)
            {
                updatedAtProperty.IsNullable = true;
            }
        }
        
        // IEntityTimeEnded - DeletedAt (nullable, no default value)
        if (typeof(IEntityTimeEnded).IsAssignableFrom(clrType))
        {
            var deletedAtProperty = entityType.FindProperty(nameof(IEntityTimeEnded.DeletedAt));
            if (deletedAtProperty != null)
            {
                deletedAtProperty.IsNullable = true;
            }
        }
    }
    
    private static void ConfigureUserAuditProperty<TInterface>(IMutableEntityType entityType, string propertyName, bool isRequired)
    {
        var clrType = entityType.ClrType;
        if (typeof(TInterface).IsAssignableFrom(clrType))
        {
            var property = entityType.FindProperty(propertyName);
            if (property != null)
            {
                property.IsNullable = !isRequired;
            }
        }
    }

    private static void ConfigureUserAuditProperty(IMutableEntityType entityType, Type genericInterfaceType, string propertyName, bool isRequired)
    {
        var clrType = entityType.ClrType;
        var interfaces = clrType.GetInterfaces();
    
        var hasGenericInterface = interfaces.Any(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == genericInterfaceType);
    
        if (hasGenericInterface)
        {
            var property = entityType.FindProperty(propertyName);
            if (property != null)
            {
                property.IsNullable = !isRequired;
            }
        }
    }
}