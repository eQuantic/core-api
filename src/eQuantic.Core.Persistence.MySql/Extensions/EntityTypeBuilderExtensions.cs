using System.Linq.Expressions;
using eQuantic.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eQuantic.Core.Persistence.MySql.Extensions;

public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Has the default primary key column name using the specified entity type builder
    /// </summary>
    /// <typeparam name="TEntity">The entity</typeparam>
    /// <typeparam name="TProperty">The property</typeparam>
    /// <param name="entityTypeBuilder">The entity type builder</param>
    /// <param name="expression">The expression</param>
    /// <returns>The entity type builder</returns>
    public static EntityTypeBuilder<TEntity> HasDefaultPrimaryKeyColumnName<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder, 
        Expression<Func<TEntity, TProperty>> expression) 
        where TEntity : class
    {
        entityTypeBuilder.Property(expression).HasColumnName($"{typeof(TEntity).Name.TrimEnd("Data")}Id");
        return entityTypeBuilder;
    }
}