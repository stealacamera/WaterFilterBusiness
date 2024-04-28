using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs.Inventory;

public abstract record InventoryRequest
{
    public int Id { get; set; }
    public InventoryItem Tool { get; set; }
    public int Quantity { get; set; }
    public InventoryRequestStatus Status { get; set; }

    public string? RequestNote { get; set; }
    public string? ConclusionNote { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ConcludedAt { get; set; }
}

public record InventoryRequest_AddRequestModel
{
    [Required]
    public int ToolId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [StringLength(210)]
    public string? RequestNote { get; set; }
}

public record InventoryRequest_PatchRequestModel
{
    [Required]
    public InventoryRequestStatus Status { get; set; }

    [StringLength(210)]
    public string? ConclusionNote { get; set; }
}

public record TechnicianInventoryRequest : InventoryRequest
{
    public User_BriefDescription Technician;
}

public record SmallInventoryRequest : InventoryRequest
{
    public User_BriefDescription Requester;
}