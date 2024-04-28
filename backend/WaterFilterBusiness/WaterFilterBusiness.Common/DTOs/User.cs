using System.ComponentModel.DataAnnotations;

namespace WaterFilterBusiness.Common.DTOs;

public record User_BriefDescription
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

public record LoginCredentials
{
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }
}

public record User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}

public record UserAddRequestModel
{
    [Required]
    [StringLength(35)]
    public string Name { get; set; }

    [Required]
    [StringLength(35)]
    public string Surname { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }
}