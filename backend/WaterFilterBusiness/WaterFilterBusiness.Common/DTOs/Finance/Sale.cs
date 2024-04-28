using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.Common.DTOs.Finance;

public record Sale
{
    public ClientMeeting_BriefDescription Meeting { get; set; }
    public PaymentType PaymentType { get; set; }

    public decimal UpfrontPaymentAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNote { get; set; }
}

public record Sale_BriefDecsription
{
    public ClientMeeting_BriefDescription Meeting { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record Sale_AddRequestModel : IValidatableObject
{
    [Required]
    public int MeetingId { get; set; }

    [Required]
    public PaymentType PaymentType { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public decimal UpfrontPaymentAmount { get; set; }

    [Range(1, double.MaxValue)]
    public decimal? MonthlyInstallmentPayment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PaymentType == PaymentType.MonthlyInstallments
            && !MonthlyInstallmentPayment.HasValue)
            yield return new ValidationResult(
                "Required monthly payment amount for monthly installment",
                new[] { nameof(MonthlyInstallmentPayment) });
    }
}