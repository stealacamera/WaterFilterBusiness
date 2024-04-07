using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs;

public record ClientMeeting
{
    public int Id { get; set; }
    
    public User_BriefDescription? PhoneOperator { get; set; }
    public User_BriefDescription SalesAgent { get; set; }
    public Customer_BriefDescription Customer { get; set; }

    public DateTime ScheduledAt { get; set; }
    public MeetingOutcome? Outcome { get; set; }

    public string? InitialNotes { get; set; }
    public string? Afternotes { get; set; }
}

public record CLientMeeting_UpdateRequestModel
{
    public int Id { get; set; }
    public MeetingOutcome Outcome { get; set; }
    public string? Afternotes { get; set; }
}

public record ClientMeeting_AddRequestModel
{
    public int? PhoneOperatorId { get; set; }

    [Required]
    public int SalesAgentId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }
    
    [StringLength(210)]
    public string? InitialNotes { get; set; }
}