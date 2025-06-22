using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Persistence.PostgreSql.Options;
using eQuantic.Core.Persistence.Relational.Extensions;
using Microsoft.EntityFrameworkCore;

namespace eQuantic.Core.Persistence.PostgreSql.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyPostgreSqlDataModelConventions(
        this ModelBuilder modelBuilder,
        Action<PostgreSqlDataModelConventionOptions>? configureOptions = null)
    {
        var options = new PostgreSqlDataModelConventionOptions();
        
        modelBuilder.ApplyRelationalDataModelConventions(opt =>
        {
            opt.UseSnakeCase();
            opt.UseFullyQualifiedPrimaryKeys();

            options = new PostgreSqlDataModelConventionOptions(opt);
            
            configureOptions?.Invoke(options);
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if(!options.EntityAuditingEnabled.HasValue || !options.EntityAuditingEnabled.Value) continue;
            
            if (!typeof(IEntityTimeMark).IsAssignableFrom(clrType)) continue;
            
            var createdAtProperty = entityType.FindProperty(nameof(IEntityTimeMark.CreatedAt));
            createdAtProperty?.SetDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        }

        return modelBuilder;
    }
}