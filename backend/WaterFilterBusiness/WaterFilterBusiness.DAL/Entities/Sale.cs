using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Entities;

public class Sale : BaseEntity<int>, IValidatableObject
{
    public int MeetingId { get; set; }
    internal ClientMeeting Meeting { get; set; }

    public int PaymentTypeId { get; set; }

    public decimal UpfrontPaymentAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }
        
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNote { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UpfrontPaymentAmount > TotalAmount)
            yield return new ValidationResult(
                "Upfront payment cannot exceed the total sale amount", 
                new[] { nameof(UpfrontPaymentAmount) });
    }
}
