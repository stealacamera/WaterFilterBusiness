using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Errors;
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
            return CustomerErrors.NotFound;

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

        return new CustomerChange
        {
            Id = dbModel.Id,
            ChangedAt = dbModel.ChangedAt,
            OldAddress = dbModel.OldAddress,
            OldCity = dbModel.OldCity,
            OldFullName = dbModel.OldFullName,
            OldIsQualified = dbModel.OldIsQualified,
            OldPhoneNumber = dbModel.OldPhoneNumber,
            OldProfession = dbModel.OldProfession
        };
    }

    public async Task<Result<IList<CustomerChange>>> GetAllForCustomer(int customerId)
    {
        if (!await _utilityService.DoesCustomerExistAsync(customerId))
            return CustomerErrors.NotFound;

        return (await _workUnit.CustomerChangesRepository
                               .GetAllForCustomer(customerId))
                               .Select(e => new CustomerChange
                               {
                                   Id = e.Id,
                                   ChangedAt = e.ChangedAt,
                                   OldAddress = e.OldAddress,
                                   OldCity = e.OldCity,
                                   OldFullName = e.OldFullName,
                                   OldIsQualified = e.OldIsQualified,
                                   OldPhoneNumber = e.OldPhoneNumber,
                                   OldProfession = e.OldProfession
                               })
                               .ToList();
    }
}
