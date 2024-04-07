using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.Repository.Calls;

public interface ICustomerCallsRepository : IRepository<CustomerCall, int>
{
    Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllAsync(
        int page, int pageSize,
        DateOnly? from = null, DateOnly? to = null,
        CallOutcome? filterByOutcome = null);

    Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllForCustomerAsync(int customerId, int page, int pageSize);
    Task<int> CountByPhoneAgent(int phoneAgentId);
}

internal class CustomerCallsRepository : Repository<CustomerCall, int>, ICustomerCallsRepository
{
    public CustomerCallsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<int> CountByPhoneAgent(int phoneAgentId)
    {
        IQueryable<CustomerCall> query = _untrackedSet.Where(e => e.PhoneAgentId == phoneAgentId);
        return await query.CountAsync();
    }

    public async Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllAsync(
        int page, int pageSize, 
        DateOnly? from = null, DateOnly? to = null, 
        CallOutcome? filterByOutcome = null)
    {
        IQueryable<CustomerCall> query = _untrackedSet;

        if (from.HasValue)
            query = query.Where(e => DateOnly.FromDateTime(e.OccuredAt) >= from);

        if (to.HasValue)
            query = query.Where(e => DateOnly.FromDateTime(e.OccuredAt) <= to);

        if (filterByOutcome != null)
            query = query.Where(e => e.OutcomeId == filterByOutcome.Value);

        return await OffsetPaginatedEnumerable<CustomerCall>.CreateAsync(query, page, pageSize);
    }

    public async Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllForCustomerAsync(int customerId, int page, int pageSize)
    {
        IQueryable<CustomerCall> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        return await OffsetPaginatedEnumerable<CustomerCall>.CreateAsync(query, page, pageSize);
    }
}
