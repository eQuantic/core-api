using eQuantic.Core.Application.Entities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.SqlServer.Configurations;

/// <summary>
/// The entity time mark configuration class
/// </summary>
/// <seealso cref="IEntityTypeConfiguration{TEntity}"/>
public class EntityTimeMarkConfiguration<TEntity> : eQuantic.Core.Persistence.Configurations.EntityTimeMarkConfiguration<TEntity> 
    where TEntity : class, IEntityTimeMark
{
    /// <summary>
    /// Configures the builder
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}