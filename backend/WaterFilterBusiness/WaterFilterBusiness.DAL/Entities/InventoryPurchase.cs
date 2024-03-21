namespace WaterFilterBusiness.DAL.Entities;

public class InventoryPurchase : Entity
{
    public int ToolId { get; set; }
    public decimal Price { get; set; }

    private int _quantity;
    public int Quantity { 
        get => _quantity;
        set 
        {
            if (value <= 0)
                throw new Exception("Quantity cannot be a nonpositive integer");

            _quantity = value;
        }
    }

    public bool IsCompleted { get; set; }
    public DateTime CompletedAt { get; set; }
}
