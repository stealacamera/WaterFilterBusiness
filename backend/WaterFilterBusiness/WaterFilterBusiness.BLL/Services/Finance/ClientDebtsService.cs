﻿using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Finance;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Finance;

public interface IClientDebtsService
{
    Task<Result<IList<ClientDebt>>> CreateAllForSaleAsync(int saleId, decimal monthlyPayment);
    Task<Result<ClientDebt>> CompletePaymentAsync(int id);
    Task<CursorPaginatedList<ClientDebt, int>> GetAllAsync(
        int cursor,
        int pageSize,
        int? filterByClient = null,
        bool? filterByCompletionStatus = null);
}

internal class ClientDebtsService : Service, IClientDebtsService
{
    public ClientDebtsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<ClientDebt>> CompletePaymentAsync(int id)
    {
        var dbModel = await _workUnit.ClientDebtsRepository.GetByIdAsync(id);

        if (dbModel == null)
            return ClientDebtErrors.NotFound(nameof(id));

        dbModel.CompletedAt = DateOnly.FromDateTime(DateTime.Now);
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<IList<ClientDebt>>> CreateAllForSaleAsync(int saleId, decimal monthlyPayment)
    {
        var sale = await _utilityService.GetSaleById(saleId);

        if (sale == null)
            return SalesErrors.NotFound(nameof(saleId));
        else if (sale.PaymentType != PaymentType.MonthlyInstallments)
            return ClientDebtErrors.InvalidSalePaymentType(nameof(sale.PaymentType));
        else if (monthlyPayment >= sale.TotalAmount - sale.UpfrontPaymentAmount)
            return ClientDebtErrors.InvalidMonthlyPayment(nameof(monthlyPayment));

        var debtEntities = new List<DAL.Entities.Clients.ClientDebt>();
        int nrMonths = 1;

        for (decimal amountLeft = sale.TotalAmount - sale.UpfrontPaymentAmount; amountLeft > 0; amountLeft -= monthlyPayment, nrMonths++)
        {
            debtEntities.Add(new DAL.Entities.Clients.ClientDebt
            {
                Amount = monthlyPayment,
                DeadlineAt = DateOnly.FromDateTime(sale.CreatedAt.AddMonths(nrMonths)),
                SaleId = saleId
            });

        }

        await _workUnit.ClientDebtsRepository.AddRangeAsync(debtEntities.ToArray());
        await _workUnit.SaveChangesAsync();

        return debtEntities.Select(ConvertEntityToModel).ToList();
    }

    public async Task<CursorPaginatedList<ClientDebt, int>> GetAllAsync(
        int cursor,
        int pageSize,
        int? filterByClient = null,
        bool? filterByCompletionStatus = null)
    {
        var debts = await _workUnit.ClientDebtsRepository
                                   .GetAllAsync(
                                        cursor, pageSize,
                                        filterByClient,
                                        filterByCompletionStatus);

        return new CursorPaginatedList<ClientDebt, int>
        {
            Cursor = debts.Cursor,
            Values = debts.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    private ClientDebt ConvertEntityToModel(DAL.Entities.Clients.ClientDebt entity)
    {
        return new ClientDebt
        {
            Id = entity.Id,
            Sale = new Sale_BriefDecsription { Meeting = new ClientMeeting_BriefDescription { Id = entity.SaleId } },
            Amount = entity.Amount,
            DeadlineAt = entity.DeadlineAt,
            CompletedAt = entity.CompletedAt
        };
    }
}
