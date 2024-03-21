namespace WaterFilterBusiness.DAL.Entities
{
    public class Sale : Entity
    {
        public int MeetingId { get; set; }
        public bool IsVerified { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AuthenticatedAt { get; set; }
        public string AuthenticationNote { get; set; }
    }
}
