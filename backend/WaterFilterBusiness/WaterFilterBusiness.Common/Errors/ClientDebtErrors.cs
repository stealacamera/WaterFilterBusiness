using FluentResults;

namespace WaterFilterBusiness.Common.Errors;

public static class ClientDebtErrors
{
    public static Error NotFound = GeneralErrors.NotFoundError("Client debt");
    public static Error InvalidSalePaymentType = new("Cannot create debts for a non-monthly-installment paid sale");
    public static Error InvalidMonthlyPayment = new("Invalid monthly payment amount: amount should be lower than the remaining debt owned");
}
