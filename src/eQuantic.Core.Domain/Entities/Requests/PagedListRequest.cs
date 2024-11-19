using eQuantic.Linq.Filter;
using eQuantic.Linq.Sorter;
using Microsoft.AspNetCore.Mvc;

namespace eQuantic.Core.Domain.Entities.Requests;

/// <summary>
/// The paged list request class
/// </summary>
public class PagedListRequest<TEntity> : BasicRequest
{
    /// <summary>
    /// Gets or sets the value of the page index
    /// </summary>
    [FromQuery]
    public int? PageIndex { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the page size
    /// </summary>
    [FromQuery]
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the filtering
    /// </summary>
    [FromQuery]
    public FilteringCollection? FilterBy { get; set; }
    
    /// <summary>
    /// Gets or sets the value of the sorting
    /// </summary>
    [FromQuery]
    public ISorting[]? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the include fields
    /// </summary>
    [FromQuery]
    public string[]? IncludeFields { get; set; }
    
    public PagedListRequest()
    {
    }

    public PagedListRequest(
        int? pageIndex, 
        int? pageSize, 
        IFiltering[]? filterBy, 
        ISorting[]? orderBy, 
        string[]? includeFields = null)
    {
        if (pageIndex.HasValue) PageIndex = pageIndex;
        if (pageSize.HasValue) PageSize = pageSize;
        if (filterBy != null) FilterBy = new FilteringCollection(filterBy);
        if (orderBy != null) OrderBy = orderBy;
        if (includeFields != null) IncludeFields = includeFields;
    }
}

/// <summary>
/// The referenced paged list request class
/// </summary>
public class PagedListRequest<TEntity, TReferenceKey> : PagedListRequest<TEntity>, IReferencedRequest<TReferenceKey>
{
    private TReferenceKey? _referenceId;

    public PagedListRequest()
    {
    }
    
    public PagedListRequest(
        TReferenceKey referenceId, 
        int? pageIndex, 
        int? pageSize, 
        IFiltering[]? filtering, 
        ISorting[]? sorting, 
        string[]? includeFields = null)
        : base(pageIndex, pageSize, filtering, sorting, includeFields)
    {
        _referenceId = referenceId;
    }
    
    public void SetReferenceId(TReferenceKey referenceId)
    {
        _referenceId = referenceId;
    }

    public TReferenceKey? GetReferenceId()
    {
        return _referenceId;
    }
}