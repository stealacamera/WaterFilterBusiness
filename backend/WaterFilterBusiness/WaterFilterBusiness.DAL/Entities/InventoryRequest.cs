namespace WaterFilterBusiness.DAL.Entities
{
    public class InventoryRequest : Entity
    {
        public int ToolId { get; set; }
        public InventoryRequestStatus Status { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ConcludedAt { get; set; }
        public string RequestNote { get; set; }
        public string ConclusionNote { get; set; }
    }
}
