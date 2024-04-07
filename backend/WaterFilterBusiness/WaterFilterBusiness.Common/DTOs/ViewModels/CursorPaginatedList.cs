namespace WaterFilterBusiness.Common.DTOs.ViewModels;

public class CursorPaginatedList<TValue, TCursor>
{
    public TCursor Cursor { get; set; }
    public IList<TValue> Values { get; set; } = new List<TValue>();
}
