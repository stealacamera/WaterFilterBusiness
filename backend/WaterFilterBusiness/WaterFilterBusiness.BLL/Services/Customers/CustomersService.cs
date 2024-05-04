using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Customers;

public interface ICustomersService
{
    Task<Result<Customer>> GetByIdAsync(int id);
    Task<Result<IList<Customer>>> CreateRangeAsync(Customer_AddRequestModel[] customers);
    Task<Result<Customer>> UpdateAsync(int id, Customer_UpdateRequestModel customer);
    Task<OffsetPaginatedList<Customer>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterWithCalls = true,
        bool? filterByRedListed = true);
}

internal class CustomersService : Service, ICustomersService
{
    public CustomersService(
        IWorkUnit workUnit,
        IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<Customer>> UpdateAsync(int id, Customer_UpdateRequestModel customer)
    {
        var dbModel = await _workUnit.CustomersRepository.GetByIdAsync(id);

        if (dbModel == null)
            return CustomerErrors.NotFound(nameof(id));

        if (IsUpdateEmpty(customer))
            return CustomerErrors.EmptyUpdate;

        if (!IsUpdateChanged(customer, dbModel))
            return GeneralErrors.UnchangedUpdate;

        if (customer.RedListedAt.HasValue && await _utilityService.DoesCustomerHaveAScheduledCallAsync(id))
            return CustomerErrors.CannotRedlist_ExistingScheduledCalls(nameof(id));

        try
        {
            dbModel = UpdateEntity(customer, dbModel);
            await _workUnit.SaveChangesAsync();

            return ConvertEntityToModel(dbModel);
        }
        catch (Exception ex)
        {
            if(ex is DbUpdateException dbUpdateEx)
            {
                if(dbUpdateEx.InnerException != null && dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        return CustomerErrors.UniqueConstraintFailed;
                }
            }

            throw;
        }
    }

    private DAL.Entities.Clients.Customer UpdateEntity(Customer_UpdateRequestModel update, DAL.Entities.Clients.Customer entity)
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

    private bool IsUpdateChanged(Customer_UpdateRequestModel updatedCustomer, DAL.Entities.Clients.Customer originalCustomer)
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
        bool? filterWithCalls = true,
        bool? filterByRedListed = true)
    {
        var customers = await _workUnit.CustomersRepository
                                       .GetAllAsync(page, pageSize, filterWithCalls, filterByRedListed);

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

        return dbModel == null
               ? CustomerErrors.NotFound(nameof(id))
               : ConvertEntityToModel(dbModel);
    }

    public async Task<Result<IList<Customer>>> CreateRangeAsync(Customer_AddRequestModel[] customers)
    {
        var dbModels = customers.Select(e => new DAL.Entities.Clients.Customer
                                    {
                                        Address = e.Address,
                                        City = e.City,
                                        FullName = e.FullName,
                                        IsQualified = e.IsQualified,
                                        PhoneNumber = e.PhoneNumber,
                                        Profession = e.Profession,
                                    })
                                .ToArray();

        try
        {
            await _workUnit.CustomersRepository.AddRangeAsync(dbModels);
            await _workUnit.SaveChangesAsync();

            return dbModels.Select(e => new Customer
            {
                Id = e.Id,
                Address = e.Address,
                City = e.City,
                FullName = e.FullName,
                IsQualified = e.IsQualified,
                PhoneNumber = e.PhoneNumber,
                Profession = e.Profession
            })
                           .ToList();
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null && dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        return CustomerErrors.UniqueConstraintFailed;
                }
            }

            throw;
        }
    }

    private Customer ConvertEntityToModel(DAL.Entities.Clients.Customer entity)
    {
        return new Customer
        {
            Id = entity.Id,
            FullName = entity.FullName,
            PhoneNumber = entity.PhoneNumber,
            Profession = entity.Profession,
            Address = entity.Address,
            City = entity.City,
            RedListedAt = entity.RedListedAt,
            IsQualified = entity.IsQualified
        };
    }
}
