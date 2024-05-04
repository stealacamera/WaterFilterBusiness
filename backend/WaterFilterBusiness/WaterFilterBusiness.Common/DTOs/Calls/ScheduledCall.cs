using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs.Calls;

public record ScheduledCall
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Customer_BriefDescription Customer { get; set; }
    public User_BriefDescription PhoneAgent { get; set; }
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