using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Customers;

public interface ICustomerChangesService
{
    Task<Result<CustomerChange>> CreateAsync(Customer currentCustomer, Customer updatedCustomer);
    Task<Result<IList<CustomerChange>>> GetAllForCustomer(int customerId);
}

internal class CustomerChangesService : Service, ICustomerChangesService
{
    public CustomerChangesService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<CustomerChange>> CreateAsync(Customer currentCustomer, Customer updatedCustomer)
    {
        if (!await _utilityService.DoesCustomerExistAsync(currentCustomer.Id))
            return CustomerErrors.NotFound(nameof(currentCustomer.Id));

        if (IsUpdateEmpty(updatedCustomer))
            return GeneralErrors.EmptyUpdate("customer");

        if (currentCustomer.Equals(updatedCustomer))
            return GeneralErrors.UnchangedUpdate;

        var dbModel = await _workUnit.CustomerChangesRepository
                                     .AddAsync(new DAL.Entities.Clients.CustomerChange
                                     {
                                         ChangedAt = DateTime.Now,
                                         CustomerId = updatedCustomer.Id,
                                         OldAddress = updatedCustomer.Address,
                                         OldCity = updatedCustomer.City,
                                         OldFullName = updatedCustomer.FullName,
                                         OldIsQualified = updatedCustomer.IsQualified,
                                         OldPhoneNumber = updatedCustomer.PhoneNumber,
                                         OldProfession = updatedCustomer.Profession
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<Result<IList<CustomerChange>>> GetAllForCustomer(int customerId)
    {
        if (!await _utilityService.DoesCustomerExistAsync(customerId))
            return CustomerErrors.NotFound(nameof(customerId));

        return (await _workUnit.CustomerChangesRepository
                               .GetAllForCustomer(customerId))
                               .Select(ConvertEntityToModel)
                               .ToList();
    }

    private CustomerChange ConvertEntityToModel(DAL.Entities.Clients.CustomerChange entity)
    {
        return new CustomerChange
        {
            Id = entity.Id,
            ChangedAt = entity.ChangedAt,
            OldAddress = entity.OldAddress,
            OldCity = entity.OldCity,
            OldFullName = entity.OldFullName,
            OldIsQualified = entity.OldIsQualified,
            OldPhoneNumber = entity.OldPhoneNumber,
            OldProfession = entity.OldProfession
        };
    }
}
