namespace WaterFilterBusiness.DAL.Entities.Clients;

public class ClientDebt : StrongEntity
{
    public int SaleId { get; set; }
    internal Sale Sale { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DeadlineAt { get; set; }
    public bool IsCompleted { get; set; }
}
