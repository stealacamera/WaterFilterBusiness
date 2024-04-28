using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository.Customers;

public interface ICustomerChangesRepository : ISimpleRepository<CustomerChange, int>
{
    Task<IEnumerable<CustomerChange>> GetAllForCustomer(int customerId);
}

internal class CustomerChangesRepository : SimpleRepository<CustomerChange, int>, ICustomerChangesRepository
{
    public CustomerChangesRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<CustomerChange>> GetAllForCustomer(int customerId)
    {
        IQueryable<CustomerChange> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        query = query.OrderByDescending(e => e.ChangedAt);

        return await query.ToListAsync();
    }
}
