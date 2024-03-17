namespace WaterFilterBusiness.DAL.Entities;

public class InventoryItem : Entity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime DeletedAt { get; set; }
}

public abstract class InventoryTypeItem : Entity<int>
{
    public int ToolId { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
                throw new Exception("Quantity cannot be a negative integer");
        }
    }
}

public class BigInventoryItem : InventoryTypeItem
{
}

public class SmallInventoryItem : InventoryTypeItem
{
}

public class TechnicianInventoryItem : InventoryTypeItem
{
    public int TechnicianId { get; set; }
}