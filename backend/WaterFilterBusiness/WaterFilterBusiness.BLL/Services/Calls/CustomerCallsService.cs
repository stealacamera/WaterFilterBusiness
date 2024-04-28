using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Calls;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Calls;

public interface ICustomerCallsService
{
    Task<Result<CustomerCall>> CreateAsync(CustomerCall_AddRequestModel customerCall);
    Task<Result<OffsetPaginatedList<CustomerCall>>> GetCallHistoryForCustomerAsync(int customerId, int page, int pageSize);

    Task<OffsetPaginatedList<CustomerCall>> GetAllAsync(
        int page, int pageSize,
        DateOnly? from = null, DateOnly? to = null,
        CallOutcome? filterByOutcome = null);
}

internal class CustomerCallsService : Service, ICustomerCallsService
{
    public CustomerCallsService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<CustomerCall>> CreateAsync(CustomerCall_AddRequestModel customerCall)
    {
        if (!await _utilityService.DoesCustomerExistAsync(customerCall.CustomerId))
            return CustomerErrors.NotFound;

        if (!await _utilityService.IsUserInRoleAsync(customerCall.PhoneAgentId, Role.PhoneOperator))
            return CallErrors.NotPhoneAgent;

        var dbModel = await _workUnit.CustomerCallsRepository
                                     .AddAsync(new DAL.Entities.Clients.CustomerCall
                                     {
                                         CustomerId = customerCall.CustomerId,
                                         OccuredAt = customerCall.OccuredAt,
                                         OutcomeId = customerCall.Outcome.Value,
                                         PhoneAgentId = customerCall.PhoneAgentId
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<CustomerCall>> GetAllAsync(
        int page, int pageSize, 
        DateOnly? from = null, DateOnly? to = null, 
        CallOutcome? filterByOutcome = null)
    {
        var result = await _workUnit.CustomerCallsRepository
                                    .GetAllAsync(page, pageSize, from, to, filterByOutcome);

        return new OffsetPaginatedList<CustomerCall>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<OffsetPaginatedList<CustomerCall>>> GetCallHistoryForCustomerAsync(int customerId, int page, int pageSize)
    {
        if (!await _utilityService.DoesCustomerExistAsync(customerId))
            return CustomerErrors.NotFound;

        var result = await _workUnit.CustomerCallsRepository.GetAllForCustomerAsync(customerId, page, pageSize);

        return new OffsetPaginatedList<CustomerCall>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    private CustomerCall ConvertEntityToModel(DAL.Entities.Clients.CustomerCall entity)
    {
        return new CustomerCall
        {
            OccuredAt = entity.OccuredAt,
            Outcome = CallOutcome.FromValue(entity.OutcomeId),
            PhoneAgent = new User_BriefDescription { Id = entity.PhoneAgentId }
        };
    }
}
