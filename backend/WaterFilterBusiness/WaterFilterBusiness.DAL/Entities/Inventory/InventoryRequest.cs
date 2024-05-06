namespace WaterFilterBusiness.DAL.Entities.Inventory;

public class InventoryRequest : StrongEntity
{
    public int ToolId { get; set; }
    public int StatusId { get; set; }
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string? RequestNote { get; set; }

    public DateTime ConcludedAt { get; set; }
    public string? ConclusionNote { get; set; }
}

public class SmallInventoryRequest : BaseEntity<int>
{
    public int InventoryRequestId { get; set; }
    internal InventoryRequest InventoryRequest { get; set; }
    public int RequesterId { get; set; }
}

public class TechnicianInventoryRequest : BaseEntity<int>
{
    public int InventoryRequestId { get; set; }
    internal InventoryRequest InventoryRequest { get; set; }
    public int TechnicianId { get; set; }
}