using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs;

public record Commission
{
    public int Id { get; set; }
    public decimal Amount { get; set; }

    public CommissionType CommissionType { get; set; } = null!;
    public string Reason { get; set; } = null!;

    public User_BriefDescription Worker { get; set; } = null!;

    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

public record Commission_BriefDescription
{
    public int Id { get; set; }

    public decimal Amount { get; set; }
    public CommissionType CommissionType { get; set; } = null!;

    public User_BriefDescription Worker { get; set; } = null!;
}

public record CommissionRequest
{
    public Commission_BriefDescription Commission { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}