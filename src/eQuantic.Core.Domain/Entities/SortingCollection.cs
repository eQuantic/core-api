using System.Reflection;
using eQuantic.Linq.Web;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Domain.Entities;

/// <summary>
/// Typed sort collection bound from the query string (<c>orderBy</c>): values are parsed with
/// the eQuantic.Linq v3 syntax (<c>path</c> or <c>path:desc</c>, comma-separated) into
/// <see cref="QuerySort{TEntity}"/> items. Binds natively in MVC (<c>TryParse</c>) and
/// Minimal APIs (<c>BindAsync</c>).
/// </summary>
/// <typeparam name="TEntity">Root entity the sort expressions are anchored on.</typeparam>
public class SortingCollection<TEntity> : List<QuerySort<TEntity>>
{
    public SortingCollection()
    {
    }

    public SortingCollection(IEnumerable<QuerySort<TEntity>> collection) : base(collection)
    {
    }

    /// <summary>Parses a single query-string value (used by MVC model binding).</summary>
    /// <param name="value">Raw ordering expression.</param>
    /// <param name="provider">Unused; required by the binding contract.</param>
    /// <param name="sortingCollection">Parsed collection, or null when the value is empty.</param>
    public static bool TryParse(string? value, IFormatProvider? provider,
        out SortingCollection<TEntity>? sortingCollection)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            sortingCollection = null;
            return false;
        }

        try
        {
            sortingCollection = new SortingCollection<TEntity>(QuerySort<TEntity>.Parse(value));
            return true;
        }
        catch (QueryStringParseException)
        {
            sortingCollection = null;
            return false;
        }
    }

    /// <summary>Binds every <c>orderBy</c> query value (used by Minimal APIs).</summary>
    /// <param name="context">Current HTTP context.</param>
    /// <param name="parameter">Bound parameter info.</param>
    public static ValueTask<SortingCollection<TEntity>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var collection = new SortingCollection<TEntity>();
        foreach (var value in context.Request.Query["orderBy"])
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            collection.AddRange(QuerySort<TEntity>.Parse(value));
        }

        return new ValueTask<SortingCollection<TEntity>?>(collection);
    }
}
