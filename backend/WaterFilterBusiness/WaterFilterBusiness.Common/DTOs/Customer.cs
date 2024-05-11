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
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }
    public string City { get; set; } = null!;

    public string Profession { get; set; } = null!;
    public bool? IsQualified { get; set; }

    public DateTime? RedListedAt { get; set; }
}

public record Customer_AddRequestModel
{
    [Required]
    [StringLength(55)]
    public string FullName { get; set; } = null!;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(70)]
    public string? Address { get; set; }

    [Required]
    [StringLength(70)]
    public string City { get; set; } = null!;

    [Required]
    [StringLength(65)]
    public string Profession { get; set; } = null!;

    public bool? IsQualified { get; set; }
}

public class Customer_UpdateRequestModel
{
    [StringLength(55)]
    public string? FullName { get; set; }

    [Phone]
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
    public Customer Customer { get; set; } = null!;
    public CustomerChange OldCustomer { get; set; } = null!;
}