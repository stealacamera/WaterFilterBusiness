namespace WaterFilterBusiness.DAL.Entities
{
    public class TechnicianInventoryRequest : Entity
    {
        public int InventoryRequestId { get; set; }
        public int TechnicianId { get; set; }
        public string Note { get; set; }
    }
}
