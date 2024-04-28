using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs;

public record InventoryPurchase_AddRequestModel
{
    [Required]
    public int ToolId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity;
}

public record InventoryPurchase
{
    public InventoryItem Tool { get; set; }
    public decimal Price { get; set; }
    public int Quantity;
    public DateTime OccurredAt { get; set; }
}