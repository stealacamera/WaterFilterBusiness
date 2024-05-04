namespace WaterFilterBusiness.DAL.Entities.Clients;

public class Customer : StrongEntity
{
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    
    public string? Address { get; set; }
    public string City { get; set; } = null!;
    
    public string Profession { get; set; } = null!;
    public bool? IsQualified { get; set; }

    internal IList<CustomerCall> CallHistory { get; set; } = new List<CustomerCall>();
    internal IList<ScheduledCall> ScheduledCalls { get; set; } = new List<ScheduledCall>();
    public DateTime? RedListedAt { get; set; }
}

public class CustomerChange : StrongEntity
{
    public int CustomerId { get; set; }
    public string? OldFullName { get; set; }
    public string? OldPhoneNumber { get; set; }

    public string? OldAddress { get; set; }
    public string? OldCity { get; set; }

    public string? OldProfession { get; set; }
    public bool? OldIsQualified { get; set; }

    public DateTime ChangedAt { get; set; }
}