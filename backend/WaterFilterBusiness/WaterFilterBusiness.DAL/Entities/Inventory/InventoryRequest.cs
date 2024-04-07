namespace WaterFilterBusiness.DAL.Entities.Inventory;

public class InventoryRequest : StrongEntity
{
    public int ToolId { get; set; }
    public int StatusId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ConcludedAt { get; set; }
    public string RequestNote { get; set; }
    public string ConclusionNote { get; set; }
}

public class SmallInventoryRequest : StrongEntity<int>
{
    public int InventoryRequestId { get; set; }
}

public class TechnicianInventoryRequest : StrongEntity<int>
{
    public int InventoryRequestId { get; set; }
    public int TechnicianId { get; set; }
}