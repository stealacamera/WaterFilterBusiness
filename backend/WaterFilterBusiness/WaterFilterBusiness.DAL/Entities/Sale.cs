namespace WaterFilterBusiness.DAL.Entities;

public class Sale : WeakEntity<int>
{
    public int MeetingId { get; set; }
    public int PaymentTypeId { get; set; }
    public decimal UpfrontPaymentAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
        
    public DateTime VerifiedAt { get; set; }
    public string VerificationNote { get; set; }
}
