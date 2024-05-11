namespace WaterFilterBusiness.DAL.Entities.Inventory;

public class InventoryItemMovement : StrongEntity
{
    public int ToolId { get; set; }
    public int Quantity { get; set; }

    public int GiverId { get; set; }
    public int FromInventoryId { get; set; }
    
    public int ReceiverId { get; set; }
    public int ToInventoryId { get; set; }
    
    public DateTime OccurredAt { get; set; }
}
