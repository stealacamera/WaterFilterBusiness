namespace WaterFilterBusiness.DAL.Entities
{
    public class SalesAgentsSchedule
    {
        public int SalesAgentId { get; set; }
        public User User { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan BeginHour { get; set; }
        public TimeSpan EndHour { get; set; }
    }
}
