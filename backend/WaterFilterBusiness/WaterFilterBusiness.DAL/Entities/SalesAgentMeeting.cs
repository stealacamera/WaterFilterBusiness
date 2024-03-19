namespace WaterFilterBusiness.DAL.Entities;

public class SalesAgentMeeting : Entity
{
    public int PhoneOperatorId { get; set; }
    public int SalesAgentId { get; set; }
    public int CustomerId { get; set; }
    public DateTime DateTime { get; set; }
    public string InitialNotes { get; set; }
    public bool IsSuccessful { get; set; }

}
