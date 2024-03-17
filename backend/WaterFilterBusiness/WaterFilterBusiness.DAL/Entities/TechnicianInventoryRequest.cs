namespace WaterFilterBusiness.DAL.Entities
{
    public class TechnicianInventoryRequest
    {
        public int InventoryRequestId { get; set; }
        public InventoryRequest InventoryRequest { get; set; }

        public int TechnicianId { get; set; }
        public User User { get; set; }
    }
}
