using eQuantic.Core.DataModel;
using eQuantic.Core.Domain.Entities;
using eQuantic.Core.Persistence.MySql.Options;
using eQuantic.Core.Persistence.Relational.Extensions;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace eQuantic.Core.Persistence.MySql.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyMySqlDataModelConventions(
        this ModelBuilder modelBuilder,
        Action<MySqlDataModelConventionOptions>? configureOptions = null)
    {
        var options = new MySqlDataModelConventionOptions();
        
        modelBuilder.ApplyRelationalDataModelConventions(opt =>
        {
            opt.UseSnakeCase();
            opt.UseFullyQualifiedPrimaryKeys();

            options = new MySqlDataModelConventionOptions(opt);
            
            configureOptions?.Invoke(options);
        });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if(!options.EntityAuditingEnabled.HasValue || !options.EntityAuditingEnabled.Value) continue;
            
            if (!typeof(IEntityTimeMark).IsAssignableFrom(clrType)) continue;
            
            var createdAtProperty = entityType.FindProperty(nameof(IEntityTimeMark.CreatedAt));
            createdAtProperty?.SetDefaultValueSql("UTC_TIMESTAMP()");
        }

        return modelBuilder;
    }
}
