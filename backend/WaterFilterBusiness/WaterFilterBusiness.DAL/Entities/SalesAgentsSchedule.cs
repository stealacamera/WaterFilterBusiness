using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.DAL.Entities;

public class SalesAgentSchedule : StrongEntity, IValidatableObject
{
    public int SalesAgentId { get; set; }
    internal User SalesAgent { get; set; }

    public int DayOfWeekId { get; set; }

    public TimeOnly BeginHour { get; set; }

    public TimeOnly EndHour { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndHour <= BeginHour)
            yield return new ValidationResult("EndHour must be greater than BeginHour.", new[] { nameof(EndHour) });

        if (BeginHour.IsBetween(new TimeOnly(8, 0), new TimeOnly(21, 30)))
            yield return new ValidationResult("BeginHour has to be between 8:00 and 21:30", new[] { nameof(BeginHour) });
    }
}

public class SalesAgentScheduleChange : StrongEntity
{
    public int ScheduleId { get; set; }
    internal SalesAgentSchedule Schedule { get; }

    public int? OldDayOfWeekId { get; set; }

    public TimeOnly? OldBeginHour { get; set; }

    public TimeOnly? OldEndHour { get; set; }

    public DateTime ChangedAt { get; set; }
}