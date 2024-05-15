using Ardalis.SmartEnum;

namespace WaterFilterBusiness.Common.Enums;

public sealed class CommissionType : SmartEnum<CommissionType>
{
    public static readonly CommissionType CustomerAddedBySalesAgent = new("Customer added by sales agent", 1);
    public static readonly CommissionType MonthlyPaymentContractCreated = new("Monthly-payment contract created", 2);
    public static readonly CommissionType UpfrontPaymentRange = new("Upfront contract payment", 3);
    public static readonly CommissionType MonthlySalesTargetReached = new("Monthly sales target reached", 4);
    public static readonly CommissionType WaterFilterInstalled = new("Water filter installed", 5);
    public static readonly CommissionType SaleCreated = new("Sale created", 6);
    
    private CommissionType(string name, int value) : base(name, value)
    {
    }
}
