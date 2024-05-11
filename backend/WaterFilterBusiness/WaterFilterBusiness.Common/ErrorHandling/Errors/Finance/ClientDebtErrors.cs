using FluentResults;

namespace WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;

public static class ClientDebtErrors
{
    public static Error NotFound(string reasonKey) => GeneralErrors.EntityNotFound(reasonKey, "Client debt");
    
    public static Error InvalidSalePaymentType(string reasonKey) 
        => new (reasonKey, new Error("Cannot create debts for a non-monthly-installment paid sale"));
    
    public static Error InvalidMonthlyPayment(string reasonKey) 
        => new (reasonKey, new Error("Invalid monthly payment amount: amount should be lower than the remaining debt owned"));
}
