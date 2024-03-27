namespace WaterFilterBusiness.DAL.Entities
{
    public class InventoryItemMovement : Entity
    {
        public int ToolId { get; set; }
        public int Quantity { get; set; }
        public InventoryMovementStatus FromInventory { get; set; }
        public InventoryMovementStatus ToInventory { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
