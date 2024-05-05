using System;

namespace WaterFilterBusiness.DAL.Repository;

public interface ICommissionRepository : IRepository<Commission, int>{
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
       int paginationCursor,
       int pageSize);
}

internal class CommissionRepository : Repository<Commission, int>, ICommissionRepository
{
    public CommissionRepository(AppDbContext dbContext) : base(dbContext)
    { 
    }
    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
       int paginationCursor,
       int pageSize, DateOnly Accepted = null)
    {
        IQueyable<Commission> query = _untrackedSet.OrderByDescending(e => e.workerID);

        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
        
    }

}