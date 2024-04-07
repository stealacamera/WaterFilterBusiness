using FluentResults;
using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Customers;

public interface ICustomersService
{
    Task<Result<Customer>> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task<IList<Customer>> CreateRangeAsync(IList<Customer> customers);
    Task<Result<Customer>> UpdateAsync(int id, CustomerUpdateRequestModel customer);
    Task<OffsetPaginatedList<Customer>> GetAllAsync(int page, int pageSize, bool excludeWithCalls = true, bool excludeRedListed = true);
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
                                     .AddAsync(new DAL.Entities.Customer
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

        return new Customer
        {
            Id = dbModel.Id,
            Address = dbModel.Address,
            City = dbModel.City,
            FullName = dbModel.FullName,
            IsQualified = dbModel.IsQualified,
            PhoneNumber = dbModel.PhoneNumber,
            Profession = dbModel.Profession,
            RedListedAt = dbModel.RedListedAt
        };
    }

    private DAL.Entities.Customer UpdateEntity(CustomerUpdateRequestModel update, DAL.Entities.Customer entity)
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

    private bool IsUpdateChanged(CustomerUpdateRequestModel updatedCustomer, DAL.Entities.Customer originalCustomer)
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
        bool excludeWithCalls = true,
        bool excludeRedListed = true)
    {
        var customers = await _workUnit.CustomersRepository
                                       .GetAllAsync(page, pageSize, excludeWithCalls, excludeRedListed);

        return new OffsetPaginatedList<Customer>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = customers.TotalCount,
            Values = customers.Values
                              .Select(e => new Customer
                              {
                                  Id = e.Id,
                                  Address = e.Address,
                                  City = e.City,
                                  FullName = e.FullName,
                                  IsQualified = e.IsQualified,
                                  PhoneNumber = e.PhoneNumber,
                                  Profession = e.Profession,
                                  RedListedAt = e.RedListedAt
                              })
                              .ToList()
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
        var dbModels = customers.Select(e => new DAL.Entities.Customer
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
            Values = result.Values
                           .Select(e => new Customer
                           {
                               Id = e.Id,
                               FullName = e.FullName,
                               Address = e.Address,
                               City = e.City,
                               PhoneNumber = e.PhoneNumber,
                               Profession = e.Profession,
                               IsQualified = e.IsQualified,
                               RedListedAt = e.RedListedAt
                           })
                           .ToList()
        };
    }
}
