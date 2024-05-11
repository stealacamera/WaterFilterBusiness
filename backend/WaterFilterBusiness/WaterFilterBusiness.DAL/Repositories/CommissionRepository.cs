using System;

namespace WaterFilterBusiness.DAL.Repository;

public interface ICommissionRepository : IRepository<Commission, int>{
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
       int paginationCursor,
       int pageSize);
    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllFromOneWorkerAsync(
        int paginationCursor,
        int pageSize,
        int WorkerID)
}

internal class CommissionRepository : Repository<Commission, int>, ICommissionRepository
{
    public CommissionRepository(AppDbContext dbContext) : base(dbContext)
    { 
    }
    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
       int paginationCursor,
       int pageSize)
    {
        IQueyable<Commission> query = _untrackedSet.OrderByDescending(e => e.workerID);
        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);
        
    }

    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllFromOneWorkerAsync(
 int paginationCursor,
 int pageSize,
 int WorkerID)
    {
        IQueyable<Commission> query = _untrackedSet.Where(e => e.workerID == WorkerID);
        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);

    }

}