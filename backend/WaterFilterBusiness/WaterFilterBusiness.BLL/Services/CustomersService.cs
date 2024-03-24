using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Results;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services;

public interface ICustomersService
{
    Task<OffsetPaginatedList<Customer>> GetAllAsync(int page, int pageSize);
    Task<Result<Customer>> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Result> UpdateAsync(Customer customer);
}

internal class CustomersService : Service, ICustomersService
{
    public CustomersService(IWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        var entity = await _workUnit.CustomersRepository
                       .AddAsync(new DAL.Entities.Customer
                       {
                           FullName = customer.FullName,
                           Address = customer.Address,
                           PhoneNumber = customer.PhoneNumber
                       });

        await _workUnit.SaveChangesAsync();

        customer.Id = entity.Id;
        return customer;
    }

    public async Task<OffsetPaginatedList<Customer>> GetAllAsync(int page, int pageSize)
    {
        var paginatedResult = await _workUnit.CustomersRepository
                                             .GetAllAsync(page, pageSize);

        var values = paginatedResult.Values
                                    .Select(e => new Customer
                                    {
                                        Id = e.Id,
                                        FullName = e.FullName,
                                        Address = e.Address,
                                        PhoneNumber = e.PhoneNumber
                                    })
                                    .ToList();

        return new OffsetPaginatedList<Customer>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = paginatedResult.TotalCount,
            Values = values
        };                               
    }

    public async Task<Result<Customer>> GetByIdAsync(int id)
    {
        var entity = await _workUnit.CustomersRepository.GetByIdAsync(id);

        if(entity == null)
            return GeneralResults.NotFoundFailResult(nameof(Customer));
        
        return Result.Ok(new Customer
                            {
                                Id = id,
                                Address = entity.Address,
                                FullName = entity.FullName,
                                PhoneNumber = entity.PhoneNumber
                            });
    }

    public async Task<Result> UpdateAsync(Customer customer)
    {
        var entity = await _workUnit.CustomersRepository.GetByIdAsync(customer.Id);

        if (entity == null)
            return GeneralResults.NotFoundFailResult(nameof(Customer));

        entity.FullName = customer.FullName;
        entity.PhoneNumber = customer.PhoneNumber;
        entity.Address = customer.Address;

        await _workUnit.SaveChangesAsync();
        return Result.Ok();
    }
}
