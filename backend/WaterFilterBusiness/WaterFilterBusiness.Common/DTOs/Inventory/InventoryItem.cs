using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs;

public record InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}

public record InventoryItem_AddRequestModel
{
    [Required]
    [StringLength(45)]
    public string Name { get; set; }

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal Price { get; set; }
}

public record InventoryItem_PatchRequestModel
{
    [StringLength(45)]
    public string? Name { get; set; }

    [Range(0.0001, double.MaxValue)]
    public decimal? Price { get; set; }
}

public record InventoryTypeItem
{
    public InventoryItem Item { get; set; }
    public int Quantity { get; set; }
}

public record InventoryTypeItem_AddRequestModel
{
    public int ToolId { get; set; }
    public int Quantity { get; set; }
}