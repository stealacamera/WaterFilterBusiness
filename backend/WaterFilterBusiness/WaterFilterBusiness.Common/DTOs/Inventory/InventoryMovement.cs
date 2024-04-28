using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs;

public record InventoryMovement
{
    public int Id { get; set; }

    public InventoryItem Tool { get; set; }
    public int Quantity { get; set; }

    public User_BriefDescription Giver { get; set; }
    public InventoryType FromInventory { get; set; }

    public User_BriefDescription Receiver { get; set; }
    public InventoryType ToInventory { get; set; }

    public DateTime OccurredAt { get; set; }
}

public record InventoryMovement_AddReqestModel
{
    [Required]
    public int ToolId { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public int GiverId { get; set; }

    [Required]
    public int ReceiverId { get; set; }
}