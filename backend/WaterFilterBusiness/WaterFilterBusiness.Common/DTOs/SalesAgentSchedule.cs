using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs;

public record SalesAgentSchedule
{
    public int Id { get; set; }
    public Weekday DayOfWeek { get; set; }
    public TimeOnly BeginHour { get; set; }
    public TimeOnly EndHour { get; set; }
}

public class SalesAgentScheduleUpdate
{
    public SalesAgentSchedule UpdatedSchedule { get; set; }
    public SalesAgentScheduleChange ScheduleChange { get; set; }
}

public class SalesAgentSchedule_AddRequestModel
{
    [Required]
    [TimeRange("8:00:00", "21:30:00")]
    public TimeOnly BeginHour { get; set; }

    [Required]
    public Weekday DayOfWeek { get; set; }
}

public class SalesAgentSchedule_UpdateRequestModel
{
    public Weekday? DayOfWeek { get; set; }

    [TimeRange("8:00:00", "21:30:00")]
    public TimeOnly? BeginHour { get; set; }
}

public class SalesAgentScheduleChange
{
    public int Id { get; set; }

    public Weekday? OldDayOfWeek { get; set; }

    public TimeOnly? OldBeginHour { get; set; }
    public TimeOnly? OldEndHour { get; set; }

    public DateTime ChangedAt { get; set; }
}