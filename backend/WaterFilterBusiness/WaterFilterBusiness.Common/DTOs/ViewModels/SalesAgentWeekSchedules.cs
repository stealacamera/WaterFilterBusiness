namespace WaterFilterBusiness.Common.DTOs.ViewModels;

public class SalesAgentWeekSchedules
{
    public Enums.Weekday DayOfWeek { get; set; }
    public IList<SalesAgentSchedule> Schedules { get; set; } = new List<SalesAgentSchedule>();
}
