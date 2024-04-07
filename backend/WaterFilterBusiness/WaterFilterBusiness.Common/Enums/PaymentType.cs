using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class PaymentType : SmartEnum<PaymentType>
{
    public static readonly PaymentType MonthlyInstallments = new("Monthly payments", 1);
    public static readonly PaymentType FullUpfront = new("Full payment upfront", 2);

    private PaymentType(string name, int value) : base(name, value)
    {
    }
}
