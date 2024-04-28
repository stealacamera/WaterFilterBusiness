using FluentResults;
using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Customers;

public interface ICustomersService
{
    Task<Result<Customer>> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task<IList<Customer>> CreateRangeAsync(IList<Customer> customers);
    Task<Result<Customer>> UpdateAsync(int id, CustomerUpdateRequestModel customer);
    Task<OffsetPaginatedList<Customer>> GetAllAsync(int page, int pageSize, bool? filterClients = true, bool? filterRedListed = true);
    Task<OffsetPaginatedList<Customer>> GetAllRedListedAsync(int page, int pageSize);
}

internal class CustomersService : Service, ICustomersService
{
    public CustomersService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        var dbModel = await _workUnit.CustomersRepository
                                     .AddAsync(new DAL.Entities.Clients.Customer
                                     {
                                         FullName = customer.FullName,
                                         Address = customer.Address,
                                         City = customer.City,
                                         PhoneNumber = customer.PhoneNumber,
                                         Profession = customer.Profession,
                                         IsQualified = customer.IsQualified
                                     });

        await _workUnit.SaveChangesAsync();

        customer.Id = dbModel.Id;
        return customer;
    }

    public async Task<Result<Customer>> UpdateAsync(int id, CustomerUpdateRequestModel customer)
    {
        var dbModel = await _workUnit.CustomersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return CustomerErrors.NotFound;

        if (IsUpdateEmpty(customer))
            return CustomerErrors.EmptyUpdate;

        if (!IsUpdateChanged(customer, dbModel))
            return GeneralErrors.UnchangedUpdate;

        if (customer.RedListedAt.HasValue && await _utilityService.DoesCustomerHaveAScheduledCallAsync(id))
            return CustomerErrors.CannotRedlist_ExistingScheduledCalls;

        dbModel = UpdateEntity(customer, dbModel);
        await _workUnit.SaveChangesAsync();

        return ConvertEntityToModel(dbModel);
    }

    private DAL.Entities.Clients.Customer UpdateEntity(CustomerUpdateRequestModel update, DAL.Entities.Clients.Customer entity)
    {
        if (!update.FullName.IsNullOrEmpty())
            entity.FullName = update.FullName;

        if (!update.PhoneNumber.IsNullOrEmpty())
            entity.PhoneNumber = update.PhoneNumber;

        if (!update.Profession.IsNullOrEmpty())
            entity.Profession = update.Profession;

        if (update.IsQualified.HasValue)
            entity.IsQualified = update.IsQualified.Value;

        if (!update.Address.IsNullOrEmpty())
            entity.Address = update.Address;

        if (!update.Profession.IsNullOrEmpty())
            entity.Profession = update.Profession;

        if (!update.City.IsNullOrEmpty())
            entity.City = update.City;

        if (update.RedListedAt.HasValue)
            entity.RedListedAt = update.RedListedAt;

        return entity;
    }

    private bool IsUpdateChanged(CustomerUpdateRequestModel updatedCustomer, DAL.Entities.Clients.Customer originalCustomer)
    {
        return HasAttributeChanged(updatedCustomer.RedListedAt, originalCustomer.RedListedAt) ||
               HasAttributeChanged(updatedCustomer.PhoneNumber, originalCustomer.PhoneNumber) ||
               HasAttributeChanged(updatedCustomer.IsQualified, originalCustomer.IsQualified) ||
               HasAttributeChanged(updatedCustomer.FullName, originalCustomer.FullName) ||
               HasAttributeChanged(updatedCustomer.Address, originalCustomer.Address) ||
               HasAttributeChanged(updatedCustomer.Profession, originalCustomer.Profession) ||
               HasAttributeChanged(updatedCustomer.City, originalCustomer.City);
    }

    public async Task<OffsetPaginatedList<Customer>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterClients = true,
        bool? filterRedListed = true)
    {
        var customers = await _workUnit.CustomersRepository
                                       .GetAllAsync(page, pageSize, filterClients, filterRedListed);

        return new OffsetPaginatedList<Customer>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = customers.TotalCount,
            Values = customers.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<Customer>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.CustomersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return CustomerErrors.NotFound;

        return new Customer
        {
            Id = dbModel.Id,
            FullName = dbModel.FullName,
            Address = dbModel.Address,
            City = dbModel.City,
            PhoneNumber = dbModel.PhoneNumber,
            Profession = dbModel.Profession,
            IsQualified = dbModel.IsQualified,
            RedListedAt = dbModel.RedListedAt
        };
    }

    public async Task<IList<Customer>> CreateRangeAsync(IList<Customer> customers)
    {
        var dbModels = customers.Select(e => new DAL.Entities.Clients.Customer
        {
            Address = e.Address,
            City = e.City,
            FullName = e.FullName,
            IsQualified = e.IsQualified,
            PhoneNumber = e.PhoneNumber,
            Profession = e.Profession
        })
        .ToList();

        await _workUnit.CustomersRepository.AddRangeAsync(dbModels.ToArray());
        await _workUnit.SaveChangesAsync();

        for (int i = 0; i < customers.Count; i++)
            customers[i].Id = dbModels[i].Id;

        return customers;
    }

    public async Task<OffsetPaginatedList<Customer>> GetAllRedListedAsync(int page, int pageSize)
    {
        var result = await _workUnit.CustomersRepository.GetAllRedListedAsync(page, pageSize);

        return new OffsetPaginatedList<Customer>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    private Customer ConvertEntityToModel(DAL.Entities.Clients.Customer entity)
    {
        return new Customer
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Address = entity.Address,
            City = entity.City,
            PhoneNumber = entity.PhoneNumber,
            Profession = entity.Profession,
            IsQualified = entity.IsQualified,
            RedListedAt = entity.RedListedAt
        };
    }
}
