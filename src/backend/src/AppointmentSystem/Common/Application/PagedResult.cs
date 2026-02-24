namespace AppointmentSystem.Common.Application;

/// <summary>
/// Wraps a paged list of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public class PagedResult<T>
{
    /// <summary>The items on the current page.</summary>
    public IReadOnlyList<T> Items { get; }
    /// <summary>Current page number (1-based).</summary>
    public int Page { get; }
    /// <summary>Number of items per page.</summary>
    public int PageSize { get; }
    /// <summary>Total number of items across all pages.</summary>
    public int TotalCount { get; }
    /// <summary>Total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    /// <summary>Whether there is a next page.</summary>
    public bool HasNextPage => Page < TotalPages;
    /// <summary>Whether there is a previous page.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Initializes a new instance of <see cref="PagedResult{T}"/>.</summary>
    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    { Items = items; Page = page; PageSize = pageSize; TotalCount = totalCount; }
}
