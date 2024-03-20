using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.DAL.Entities;

public class SalesAgentSchedule : Entity
{
    public int SalesAgentId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeSpan BeginHour { get; set; }

    public TimeSpan EndHour { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndHour <= BeginHour)
        {
            yield return new ValidationResult("EndHour must be greater than BeginHour.", new[] { nameof(EndHour) });
        }
    }
}

