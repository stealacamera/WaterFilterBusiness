namespace WaterFilterBusiness.Common.DTOs.Finance;

public record ClientDebt
{
    public int Id { get; set; }
    public Sale_BriefDecsription Sale { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DeadlineAt { get; set; }
    public DateOnly? CompletedAt { get; set; }
}