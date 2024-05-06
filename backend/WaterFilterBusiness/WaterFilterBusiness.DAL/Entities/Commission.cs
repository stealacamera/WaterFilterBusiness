namespace WaterFilterBusiness.DAL.Entities;

public class Commission : StrongEntity
{
    public decimal Amount { get; set; }
    public int CommissionTypeId { get; set; }
    public string Reason { get; set; } = null!;
    public int WorkerId { get; set; }
    
    public DateTime ApprovedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

public class CommissionRequest : BaseEntity<int>
{
    public int CommissionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}