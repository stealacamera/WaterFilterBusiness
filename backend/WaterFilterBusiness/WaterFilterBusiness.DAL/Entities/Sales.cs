namespace WaterFilterBusiness.DAL.Entities
{
    public class Sale
    {
        public int MeetingId { get; set; }
        public SalesAgentMeeting Meeting { get; set; }

        public bool IsVerified { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
//TODO: add the other fields
    }
}
