namespace WaterFilterBusiness.DAL.Entities
{
    public class InventoryItemMovement : Entity
    {
        public int ToolId { get; set; }
        public int Quantity { get; set; }
        public int FromInventory { get; set; }
        public int ToInventory { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
