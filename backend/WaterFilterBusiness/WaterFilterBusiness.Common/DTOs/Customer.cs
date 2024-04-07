using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs;

public record Customer_BriefDescription
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string? Address { get; set; }
}

public record Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }

    public string? Address { get; set; }
    public string City { get; set; }

    public string Profession { get; set; }
    public bool? IsQualified { get; set; }

    public DateTime? RedListedAt { get; set; }
}

public record CustomerAddRequestModel
{
    [Required]
    [StringLength(55)]
    public string FullName { get; set; }

    [Required]
    [StringLength(30)]
    public string PhoneNumber { get; set; }

    [StringLength(70)]
    public string? Address { get; set; }

    [Required]
    [StringLength(70)]
    public string City { get; set; }

    [Required]
    [StringLength(65)]
    public string Profession { get; set; }

    public bool? IsQualified { get; set; }
}

public class CustomerUpdateRequestModel
{
    [StringLength(55)]
    public string? FullName { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(70)]
    public string? Address { get; set; }

    [StringLength(70)]
    public string? City { get; set; }

    [StringLength(65)]
    public string? Profession { get; set; }

    public bool? IsQualified { get; set; }
    public DateTime? RedListedAt { get; set; }
}

public class CustomerChange
{
    public int Id { get; set; }
    public string? OldFullName { get; set; }
    public string? OldPhoneNumber { get; set; }

    public string? OldAddress { get; set; }
    public string? OldCity { get; set; }

    public string? OldProfession { get; set; }
    public bool? OldIsQualified { get; set; }

    public DateTime ChangedAt { get; set; }
}

public class CustomerUpdate
{
    public Customer Customer { get; set; }
    public CustomerChange OldCustomer { get; set; }
}