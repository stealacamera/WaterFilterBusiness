namespace WaterFilterBusiness.Common.DTOs.ViewModels;

public class OffsetPaginatedList<T>
{
    public IList<T> Values { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
}
