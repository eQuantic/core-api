using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Persistence.Relational.Extensions;
using eQuantic.Core.Persistence.SqlServer.Options;
using Microsoft.EntityFrameworkCore;

namespace eQuantic.Core.Persistence.SqlServer.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplySqlServerDataModelConventions(
        this ModelBuilder modelBuilder,
        Action<SqlServerDataModelConventionOptions>? configureOptions = null)
    {
        var options = new SqlServerDataModelConventionOptions();
        
        modelBuilder.ApplyRelationalDataModelConventions(opt =>
        {
            opt.UsePascalCase();
            opt.UseFullyQualifiedPrimaryKeys();

            options = new SqlServerDataModelConventionOptions(opt);
            
            configureOptions?.Invoke(options);
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if(!options.EntityAuditingEnabled.HasValue || !options.EntityAuditingEnabled.Value) continue;
            
            if (!typeof(IEntityTimeMark).IsAssignableFrom(clrType)) continue;
            
            var createdAtProperty = entityType.FindProperty(nameof(IEntityTimeMark.CreatedAt));
            createdAtProperty?.SetDefaultValueSql("GETUTCDATE()");
        }

        return modelBuilder;
    }
}
