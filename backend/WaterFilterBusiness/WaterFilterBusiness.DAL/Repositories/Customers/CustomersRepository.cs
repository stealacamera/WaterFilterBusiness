using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository.Customers;

public interface ICustomersRepository : ISimpleRepository<Customer, int>
{
    Task AddRangeAsync(Customer[] customers);
    Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(int page, int pageSize, bool? filterClients = true, bool? filterRedListed = true);
    Task<OffsetPaginatedEnumerable<Customer>> GetAllRedListedAsync(int page, int pageSize);
}

internal class CustomersRepository : SimpleRepository<Customer, int>, ICustomersRepository
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
        bool? filterClients = true,
        bool? filterRedListed = true)
    {
        IQueryable<Customer> query = _untrackedSet.OrderByDescending(e => e.Id)
                                                  .ThenByDescending(e => e.IsQualified);

        // If client filtering is true, only include customers with calls
        // otherwise, exclude customers with previous/upcoming calls
        if (filterClients.HasValue)
            query = query.Where(e => 
                filterClients.Value 
                ? e.CallHistory.Any() 
                : !e.CallHistory.Any() && e.ScheduledCall != null);

        // If redlist filtering is true, exclude redlisted customers
        // otherwise, only include redlisted customers
        if (filterRedListed.HasValue)
            query = query.Where(e => 
                filterRedListed.Value 
                ? e.RedListedAt == null 
                : e.RedListedAt != null);

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
