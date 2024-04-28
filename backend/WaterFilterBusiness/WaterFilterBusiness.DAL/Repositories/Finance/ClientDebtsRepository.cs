using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities.Clients;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Finance;

public interface IClientDebtsRepository : ISimpleRepository<ClientDebt, int>
{
    Task AddRangeAsync(ClientDebt[] entities);
    Task<CursorPaginatedEnumerable<ClientDebt, int>> GetAllAsync(
        int paginationCursor, 
        int pageSize, 
        int? filterByClient = null,
        bool? filterByCompletionStatus = null);
}

internal class ClientDebtsRepository : SimpleRepository<ClientDebt, int>, IClientDebtsRepository
{
    public ClientDebtsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AddRangeAsync(ClientDebt[] entities)
    {
        await _set.AddRangeAsync(entities);
    }

    public async Task<CursorPaginatedEnumerable<ClientDebt, int>> GetAllAsync(
        int paginationCursor, 
        int pageSize, 
        int? filterByClient = null, 
        bool? filterByCompletionStatus = null)
    {
        IQueryable<ClientDebt> query = _untrackedSet.OrderByDescending(e => e.DeadlineAt);

        if (filterByClient.HasValue)
            query = query.Where(e => e.Sale.Meeting.CustomerId == filterByClient.Value);

        if (filterByCompletionStatus.HasValue)
            query = query.Where(e => e.IsCompleted == filterByCompletionStatus.Value);

        return await CursorPaginatedEnumerable<ClientDebt, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
    }
}
