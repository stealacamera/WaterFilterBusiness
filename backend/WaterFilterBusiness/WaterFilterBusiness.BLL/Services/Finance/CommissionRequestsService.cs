using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Finance;

public interface ICommissionRequestsService
{
    Task<OffsetPaginatedList<CommissionRequest>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterByCompleted = null);

    Task<Result<OffsetPaginatedList<CommissionRequest>>> GetAllFromWorkerAsync(
        int page,
        int pageSize,
        int workerId,
        bool? filterByCompleted = null);

    Task<Result<CommissionRequest>> GetByIdAsync(int id);
    Task<Result<CommissionRequest>> CreateAsync(int commissionId);
    Task<Result<CommissionRequest>> UpdateAsync(int commissionId);
}

internal class CommissionRequestsService : Service, ICommissionRequestsService
{
    public CommissionRequestsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<CommissionRequest>> CreateAsync(int commissionId)
    {
        if (!await _utilityService.DoesCommissionExistAsync(commissionId))
            return GeneralErrors.EntityNotFound(nameof(Commission));
        else if (await _utilityService.IsCommissionReleasedAsync(commissionId))
            return CommissionErrors.AlreadyReleased(nameof(commissionId));

        if (await _workUnit.CommissionRequestsRepository.GetByIdAsync(commissionId) != null)
            return CommissionErrors.ExistingRequest(nameof(commissionId));

        var dbModel = await _workUnit.CommissionRequestsRepository
                                     .AddAsync(new DAL.Entities.CommissionRequest
                                     {
                                         CommissionId = commissionId,
                                         CreatedAt = DateTime.Now
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<CommissionRequest>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterByCompleted = null)
    {
        var requests = await _workUnit.CommissionRequestsRepository
                                      .GetAllAsync(page, pageSize, filterByCompleted);

        return new OffsetPaginatedList<CommissionRequest>
        {
            Page = page,
            PageSize = page,
            TotalCount = requests.TotalCount,
            Values = requests.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<OffsetPaginatedList<CommissionRequest>>> GetAllFromWorkerAsync(
        int page,
        int pageSize,
        int workerId,
        bool? filterByCompleted = null)
    {
        var userRole = await _utilityService.GetUserRoleAsync(workerId);

        if (userRole == null || userRole != Role.PhoneOperator || userRole != Role.SalesAgent)
            return GeneralErrors.Unauthorized(nameof(workerId));

        var requests = await _workUnit.CommissionRequestsRepository
                                      .GetAllFromWorkerAsync(page, pageSize, workerId, filterByCompleted);

        return new OffsetPaginatedList<CommissionRequest>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = requests.TotalCount,
            Values = requests.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<CommissionRequest>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.CommissionRequestsRepository.GetByIdAsync(id);

        return dbModel == null
               ? GeneralErrors.EntityNotFound(nameof(CommissionRequest))
               : ConvertEntityToModel(dbModel);
    }

    public async Task<Result<CommissionRequest>> UpdateAsync(int commissionId)
    {
        var dbModel = await _workUnit.CommissionRequestsRepository.GetByIdAsync(commissionId);

        if (dbModel == null)
            return CommissionErrors.NotFound(nameof(commissionId));

        dbModel.CompletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    private CommissionRequest ConvertEntityToModel(DAL.Entities.CommissionRequest entity)
    {
        return new CommissionRequest
        {
            Commission = new Commission_BriefDescription { Id = entity.CommissionId },
            CompletedAt = entity.CompletedAt,
            CreatedAt = entity.CreatedAt
        };
    }
}
