using Microsoft.EntityFrameworkCore;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;
using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.Repository.Calls;

public interface ICustomerCallsRepository : ISimpleRepository<CustomerCall, int>
{
    Task<int> CountByPhoneAgent(int phoneAgentId);

    Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllAsync(
        int page, int pageSize,
        DateOnly? from = null, 
        DateOnly? to = null,
        CallOutcome? filterByOutcome = null);

    Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllForCustomerAsync(
        int customerId, 
        int page, 
        int pageSize);
    
    Task<IEnumerable<CustomerCall>> GetAllForPhoneAgentAsync(
        int phoneAgentId, 
        DateOnly? from = null, 
        DateOnly? to = null);

    Task<IEnumerable<CustomerCall>> GetLastestXWeekCallsForPhoneAgentAsync(
        int phoneAgentId,
        int nrWeeks);
}

internal class CustomerCallsRepository : SimpleRepository<CustomerCall, int>, ICustomerCallsRepository
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
        DateOnly? from = null, 
        DateOnly? to = null, 
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

    public async Task<OffsetPaginatedEnumerable<CustomerCall>> GetAllForCustomerAsync(
        int customerId, 
        int page, 
        int pageSize)
    {
        IQueryable<CustomerCall> query = _untrackedSet.Where(e => e.CustomerId == customerId);
        return await OffsetPaginatedEnumerable<CustomerCall>.CreateAsync(query, page, pageSize);
    }

    public async Task<IEnumerable<CustomerCall>> GetAllForPhoneAgentAsync(
        int phoneAgentId, 
        DateOnly? from = null,
        DateOnly? to = null)
    {
        IQueryable<CustomerCall> query = _untrackedSet.Where(e => e.PhoneAgentId == phoneAgentId);
        query = query.OrderBy(e => e.OccuredAt);

        if (from.HasValue)
            query = query.Where(e => e.OccuredAt >= from.Value.ToDateTime(TimeOnly.MinValue));

        if (to.HasValue)
            query = query.Where(e => e.OccuredAt <= to.Value.ToDateTime(TimeOnly.MinValue));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<CustomerCall>> GetLastestXWeekCallsForPhoneAgentAsync(
        int phoneAgentId, 
        int nrWeeks)
    {
        IQueryable<CustomerCall> query = _untrackedSet.Where(e => e.PhoneAgentId == phoneAgentId);
        query = query.OrderByDescending(e => e.OccuredAt);

        var beginDate = DateTime.Now
                                .AddDays(-1 * (nrWeeks * 7))
                                .StartOfWeek();

        query = query.Where(e => e.OccuredAt <= beginDate);
        return await query.ToListAsync();
    }
}
