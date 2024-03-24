using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs;

public record Customer
{
    [ValidateNever]
    public int Id { get; set; }

    [StringLength(55)]
    public string FullName { get; set; } = null!;

    [Phone]
    public string PhoneNumber { get; set; }

    [StringLength(70)]
    public string Address { get; set; }
}