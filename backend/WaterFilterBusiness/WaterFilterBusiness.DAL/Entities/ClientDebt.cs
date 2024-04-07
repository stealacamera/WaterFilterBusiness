namespace WaterFilterBusiness.DAL.Entities;

public class ClientDebt : StrongEntity
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DeadlineAt { get; set; }
    public bool IsCompleted { get; set; }
}
