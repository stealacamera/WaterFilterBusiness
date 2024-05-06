namespace WaterFilterBusiness.DAL.Entities.Inventory;

public class InventoryPurchase : StrongEntity
{
    public int ToolId { get; set; }
    public decimal Price { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(Quantity));

            _quantity = value;
        }
    }

    public DateTime OccurredAt { get; set; }
}
