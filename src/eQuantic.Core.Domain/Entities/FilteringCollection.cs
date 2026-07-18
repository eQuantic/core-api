using System.Linq.Expressions;
using System.Reflection;
using eQuantic.Linq.Expressions;
using eQuantic.Linq.Web;
using Microsoft.AspNetCore.Http;

namespace eQuantic.Core.Domain.Entities;

/// <summary>
/// Typed filter collection bound from the query string (<c>filterBy</c>): each value is parsed
/// with the eQuantic.Linq v3 syntax (e.g. <c>total:gt(100),status:eq(Paid)</c>) into a
/// serializable <see cref="ExpressionModel{TEntity}"/>; items combine with AND.
/// Binds natively in MVC (<c>TryParse</c>) and Minimal APIs (<c>BindAsync</c>).
/// </summary>
/// <typeparam name="TEntity">Root entity the filter expressions are anchored on.</typeparam>
public class FilteringCollection<TEntity> : List<ExpressionModel<TEntity>>
{
    public FilteringCollection()
    {
    }

    public FilteringCollection(IEnumerable<ExpressionModel<TEntity>> collection) : base(collection)
    {
    }

    /// <summary>
    /// Combines all filter models into a single typed predicate (null when empty).
    /// </summary>
    /// <param name="options">Query-string options; defaults apply when omitted.</param>
    public Expression<Func<TEntity, bool>>? ToPredicate(QueryStringOptions? options = null)
    {
        var serializer = options?.Serializer ?? ExpressionSerializer.Default;
        Expression<Func<TEntity, bool>>? predicate = null;
        foreach (var model in this)
        {
            var next = serializer.ToPredicate(model);
            predicate = predicate is null ? next : predicate.AndAlso(next);
        }

        return predicate;
    }

    /// <summary>Parses a single query-string value (used by MVC model binding).</summary>
    /// <param name="value">Raw filter expression.</param>
    /// <param name="provider">Unused; required by the binding contract.</param>
    /// <param name="filteringCollection">Parsed collection, or null when the value is empty.</param>
    public static bool TryParse(string? value, IFormatProvider? provider,
        out FilteringCollection<TEntity>? filteringCollection)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            filteringCollection = null;
            return false;
        }

        try
        {
            filteringCollection = [QueryFilter.ParseModel<TEntity>(value)];
            return true;
        }
        catch (QueryStringParseException)
        {
            filteringCollection = null;
            return false;
        }
    }

    /// <summary>Binds every <c>filterBy</c> query value (used by Minimal APIs).</summary>
    /// <param name="context">Current HTTP context.</param>
    /// <param name="parameter">Bound parameter info.</param>
    public static ValueTask<FilteringCollection<TEntity>?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var collection = new FilteringCollection<TEntity>();
        foreach (var value in context.Request.Query["filterBy"])
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            collection.Add(QueryFilter.ParseModel<TEntity>(value));
        }

        return new ValueTask<FilteringCollection<TEntity>?>(collection);
    }
}
