using Humanizer;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace eQuantic.Core.Persistence.MongoDb.Extensions;

public static class ModelBuilderExtensions
{
    public static void UseCamelCase(this ModelBuilder modelBuilder, string entitySuffix = "Data")
    {
        foreach(var entity in modelBuilder.Model.GetEntityTypes())
        {
            var entityName = entity.Name.TrimEnd(entitySuffix.ToCharArray());
            
            // Replace table names
            entity.SetCollectionName(entityName.Pluralize().Camelize());

            // Replace column names            
            foreach(var property in entity.GetProperties())
            {
                property.SetElementName(property.Name.Camelize());
            }
        }
    }
}