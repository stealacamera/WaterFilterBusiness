using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Finance;

public interface ICommissionsService
{
    Task<Result<Commission>> CreateCustomerAddedCommission(int salesAgentId, string baseCustomerFullName);
    Task<Result<Commission>> CreateMonthlyPaymentContractComission(int salesAgentId, string customerFullName);
    Task<Result<Commission>> CreateWaterFilterInstalledCommission(int technicianId, string customerFullName);

    Task<Result<Commission?>> CreateSaleCommissionForSalesAgentAsync(
        int salesAgentId,
        int contractPaymentAmount,
        int referralsCount,
        string customerFullName);

    Task<Result<Commission?>> CreateSaleCommissionForPhoneAgentAsync(
        int phoneAgentId,
        int contractPaymentAmount,
        string customerFullName);

    Task<Result<Commission?>> CreateMonthlySalesCommissionsForSalesAgentAsync(
        int salesAgentId,
        int salesNumber);

    Task<Result<Commission>> CreateMonthlySalesCommissionsForPhoneAgentAsync(
        int phoneAgentId,
        int salesNumber);

    Task<OffsetPaginatedList<Commission>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterByApproval = null,
        bool? filterByReleased = null);

    Task<Result<OffsetPaginatedList<Commission>>> GetAllForWorkerAsync(int page, int pageSize, int workerID);

    Task<Result<Commission>> GetByIdAsync(int id);
    Task<Result<Commission>> ApproveAsync(int id);
    Task<Result<Commission>> ReleaseAsync(int id);
}

internal class CommissionsService : Service, ICommissionsService
{
    public CommissionsService(IWorkUnit workUnit, IUtilityService utilityService)
        : base(workUnit, utilityService)
    {
    }

    public async Task<Result<Commission>> CreateCustomerAddedCommission(int salesAgentId, string baseCustomerFullName)
    {
        if (!await _utilityService.IsUserInRoleAsync(salesAgentId, Role.SalesAgent))
            return UserErrors.NotFound;

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = 0.5m,
                                         CommissionTypeId = CommissionType.CustomerAddedBySalesAgent.Value,
                                         Reason = $"Customer added during meeting with {baseCustomerFullName}",
                                         WorkerId = salesAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission>> CreateMonthlyPaymentContractComission(int salesAgentId, string customerFullName)
    {
        if (!await _utilityService.IsUserInRoleAsync(salesAgentId, Role.SalesAgent))
            return UserErrors.NotFound;

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = 5,
                                         CommissionTypeId = CommissionType.MonthlyPaymentContractCreated.Value,
                                         Reason = $"Set up a monthly payment contract with {customerFullName}",
                                         WorkerId = salesAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission>> CreateWaterFilterInstalledCommission(int technicianId, string customerFullName)
    {
        if (!await _utilityService.IsUserInRoleAsync(technicianId, Role.SalesAgent))
            return UserErrors.NotFound;

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = 10,
                                         CommissionTypeId = CommissionType.WaterFilterInstalled.Value,
                                         Reason = $"Technician installed water filter for customer {customerFullName}",
                                         WorkerId = technicianId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission?>> CreateSaleCommissionForSalesAgentAsync(
        int salesAgentId,
        int contractPaymentAmount,
        int referralsCount,
        string customerFullName)
    {
        if (!await _utilityService.IsUserInRoleAsync(salesAgentId, Role.SalesAgent))
            return UserErrors.NotFound;

        decimal amount;

        switch (contractPaymentAmount)
        {
            case >= 295:
                amount = 25;
                break;
            case >= 200:
                amount = 20;
                break;
            case >= 100:
                amount = 15;
                break;
            case >= 50:
                amount = 10;
                break;
            default:
                return Result.Ok();
        }

        if (referralsCount < 10)
            amount -= 5;

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = amount,
                                         CommissionTypeId = CommissionType.SaleCreated,
                                         Reason = $"Got an upfront payment of {contractPaymentAmount}"
                                                  + $"with {referralsCount} referals from {customerFullName}",
                                         WorkerId = salesAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission?>> CreateSaleCommissionForPhoneAgentAsync(
        int phoneAgentId,
        int contractPaymentAmount,
        string customerFullName)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneAgentId, Role.PhoneOperator))
            return UserErrors.NotFound;

        if (contractPaymentAmount <= 50)
            return Result.Ok();

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = 5,
                                         CommissionTypeId = CommissionType.SaleCreated,
                                         Reason = $"Booked meeting with {customerFullName}",
                                         WorkerId = phoneAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission?>> CreateMonthlySalesCommissionsForSalesAgentAsync(
        int salesAgentId,
        int salesNumber)
    {
        if (!await _utilityService.IsUserInRoleAsync(salesAgentId, Role.SalesAgent))
            return UserErrors.NotFound;

        decimal amount;

        switch (salesNumber)
        {
            case >= 7:
                amount = 160 * salesNumber;
                break;
            case >= 5:
                amount = 150 * salesNumber;
                break;
            case >= 3:
                amount = 140 * salesNumber;
                break;
            default:
                return Result.Ok();
        }

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = amount,
                                         CommissionTypeId = CommissionType.MonthlySalesTargetReached.Value,
                                         Reason = $"Completed {salesNumber} total sales for the month",
                                         WorkerId = salesAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }
    public async Task<Result<Commission>> CreateMonthlySalesCommissionsForPhoneAgentAsync(
        int phoneAgentId,
        int salesNumber)
    {
        if (!await _utilityService.IsUserInRoleAsync(phoneAgentId, Role.PhoneOperator))
            return UserErrors.NotFound;

        var dbModel = await _workUnit.CommissionsRepository
                                     .AddAsync(new DAL.Entities.Commission
                                     {
                                         Amount = 1.5m * salesNumber,
                                         CommissionTypeId = CommissionType.MonthlySalesTargetReached.Value,
                                         Reason = $"Secured {salesNumber} sales during the month",
                                         WorkerId = phoneAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<Commission>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.CommissionsRepository.GetByIdAsync(id);

        return dbModel == null
               ? CommissionErrors.NotFound(nameof(id))
               : ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<Commission>> GetAllAsync(
        int page, 
        int pageSize, 
        bool? filterByApproval = null, 
        bool? filterByReleased = null)
    {
        var result = await _workUnit.CommissionsRepository
                                    .GetAllAsync(page, pageSize, filterByApproval, filterByReleased);

        return new OffsetPaginatedList<Commission>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<OffsetPaginatedList<Commission>>> GetAllForWorkerAsync(int page, int pageSize, int workerID)
    {
        var result = await _workUnit.CommissionsRepository
                                    .GetAllForWorkerAsync(page, pageSize, workerID);

        return new OffsetPaginatedList<Commission>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }
    
    public async Task<Result<Commission>> ApproveAsync(int id)
    {
        var val = await _workUnit.CommissionsRepository.GetByIdAsync(id);

        if (val == null)
            return CommissionErrors.NotFound(nameof(id));

        val.ApprovedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(val);
    }

    public async Task<Result<Commission>> ReleaseAsync(int id)
    {
        var val = await _workUnit.CommissionsRepository.GetByIdAsync(id);

        if (val == null)
            return CommissionErrors.NotFound(nameof(id));
        else if (val.ApprovedAt == null)
            return CommissionErrors.Unapproved(nameof(id));

        val.ReleasedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(val);
    }

    private Commission ConvertEntityToModel(DAL.Entities.Commission entity)
    {
        return new Commission
        {
            Id = entity.Id,
            Amount = entity.Amount,
            CommissionType = CommissionType.FromValue(entity.CommissionTypeId),
            Reason = entity.Reason,
            ReleasedAt = entity.ReleasedAt,
            ApprovedAt = entity.ApprovedAt,
            Worker = new User_BriefDescription { Id = entity.Id }
        };
    }
}