using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repositories;

public interface ICustomersRepository : IRepository<Customer, int>
{
    Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(int page, int pageSize);
}

internal sealed class CustomersRepository : Repository<Customer, int>, ICustomersRepository
{

    public CustomersRepository(AppDbContext dbContext) : base(dbContext)
    {        
    }

    public async Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(int page, int pageSize)
    {
        var query = _untrackedSet.OrderByDescending(e => e.Id);

        return await OffsetPaginatedEnumerable<Customer>.CreateAsync(query, page, pageSize);
    }
}
