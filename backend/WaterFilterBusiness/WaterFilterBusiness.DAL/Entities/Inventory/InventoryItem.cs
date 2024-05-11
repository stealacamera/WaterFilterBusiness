namespace WaterFilterBusiness.DAL.Entities.Inventory;

public class InventoryItem : StrongEntity
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public abstract class InventoryTypeItem : BaseEntity<int>
{
    public int ToolId { get; set; }
    protected internal InventoryItem Tool { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Quantity));

            _quantity = value;
        }
    }
}

public class BigInventoryItem : InventoryTypeItem { }
public class SmallInventoryItem : InventoryTypeItem { }

public class TechnicianInventoryItem : CompositeEntity<int, int>
{
    public int TechnicianId { get; set; }

    public int ToolId { get; set; }
    internal InventoryItem Tool { get; set; }
        
    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Quantity));

            _quantity = value;
        }
    }
}