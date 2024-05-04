using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository.Customers;

public interface ICustomersRepository : ISimpleRepository<Customer, int>
{
    Task AddRangeAsync(Customer[] customers);
    Task<OffsetPaginatedEnumerable<Customer>> GetAllAsync(
        int page, 
        int pageSize,
        bool? filterWithCalls = true,
        bool? filterByRedListed = true);
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
        bool? filterWithCalls = true,
        bool? filterByRedListed = true)
    {
        IQueryable<Customer> query = _untrackedSet.OrderByDescending(e => e.Id)
                                                  .ThenByDescending(e => e.IsQualified);

        // If with-call filtering is enabled
        // a truthy value gets only customers with previous call history or a scheduled call
        // a false value gets only customers with no calls set-up
        if (filterWithCalls.HasValue)
            query = query.Where(e => filterWithCalls.Value
                                     ? e.CallHistory.Any() || e.ScheduledCalls.Any()
                                     : !e.CallHistory.Any() && !e.ScheduledCalls.Any());

        // If red-listed filtering is enabled
        // a truthy value gets only red-listed customers
        // a false value gets only non-redlisted customers
        if (filterByRedListed.HasValue)
            query = query.Where(e => filterByRedListed.Value
                                     ? e.RedListedAt != null
                                     : e.RedListedAt == null);

        return await OffsetPaginatedEnumerable<Customer>.CreateAsync(query, page, pageSize);
    }
}
