namespace WaterFilterBusiness.DAL.Entities
{
    public class TechnicianInventoryRequest : Entity<int>
    {
        public int InventoryRequestId { get; set; }
        public int TechnicianId { get; set; }
    }
}
