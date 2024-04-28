namespace WaterFilterBusiness.DAL.Entities.Clients;

public class Customer : StrongEntity
{
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }

    public string? Address { get; set; }
    public string City { get; set; }

    public string Profession { get; set; }
    public bool? IsQualified { get; set; }

    internal ICollection<CustomerCall> CallHistory { get; set; }
    internal ScheduledCall ScheduledCall { get; set; }
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