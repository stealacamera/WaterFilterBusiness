using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository.Customers;

public interface ICustomersRepository : IRepository<Customer, int>
{
    Task AddRangeAsync(Customer[] customers);
    Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(int page, int pageSize, bool excluseWithCalls = true, bool excludeRedListed = true);
    Task<OffsetPaginatedEnumerable<Customer>> GetAllRedListedAsync(int page, int pageSize);
}

internal class CustomersRepository : Repository<Customer, int>, ICustomersRepository
{
    public CustomersRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AddRangeAsync(Customer[] customers)
    {
        await _set.AddRangeAsync(customers);
    }

    public async Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(
        int page,
        int pageSize,
        bool excludeWithCalls = true,
        bool excludeRedListed = true)
    {
        IQueryable<Customer> query = _untrackedSet.OrderByDescending(e => e.Id)
                                                  .ThenByDescending(e => e.IsQualified);

        if (excludeWithCalls)
            query = query.Where(e => !e.CallHistory.Any() && e.ScheduledCall != null);

        if (excludeRedListed)
            query = query.Where(e => e.RedListedAt == null);

        return await OffsetPaginatedEnumerable<Customer>.CreateAsync(query, page, pageSize);
    }

    public async Task<OffsetPaginatedEnumerable<Customer>> GetAllRedListedAsync(int page, int pageSize)
    {
        IQueryable<Customer> query = _untrackedSet.OrderByDescending(e => e.Id)
                                                  .ThenByDescending(e => e.IsQualified);

        query = _untrackedSet.Where(e => e.RedListedAt != null);
        return await OffsetPaginatedEnumerable<Customer>.CreateAsync(query, page, pageSize);
    }
}
