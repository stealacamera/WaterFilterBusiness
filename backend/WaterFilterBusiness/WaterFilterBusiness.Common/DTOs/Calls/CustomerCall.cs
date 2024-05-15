using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs.Calls;

public record CustomerCall
{
    public User_BriefDescription PhoneAgent { get; set; }
    public Customer_BriefDescription Customer { get; set; }

    public CallOutcome Outcome { get; set; }
    public DateTime OccuredAt { get; set; }
}

public record CustomerCall_AddRequestModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PhoneAgentId { get; set; }

    [Required]
    public CallOutcome Outcome { get; set; }

    [Required]
    public DateTime OccuredAt { get; set; }
}