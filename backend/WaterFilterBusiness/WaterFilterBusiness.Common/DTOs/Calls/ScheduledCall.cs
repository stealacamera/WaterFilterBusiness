using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs.Calls;

public record ScheduledCall
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PhoneAgentId { get; set; }
    public DateTime ScheduledAt { get; set; }
}

public record ScheduledCall_AddRequestModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PhoneAgentId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }
}