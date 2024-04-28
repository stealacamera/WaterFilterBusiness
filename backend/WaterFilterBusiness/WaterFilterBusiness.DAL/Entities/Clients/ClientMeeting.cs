namespace WaterFilterBusiness.DAL.Entities.Clients;

public class ClientMeeting : StrongEntity
{
    public int? PhoneOperatorId { get; set; }
    public int SalesAgentId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int? MeetingOutcomeId { get; set; }

    public string? InitialNotes { get; set; }
    public string? Afternotes { get; set; }
}
