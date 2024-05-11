using System;

namespace WaterFilterBusiness.DAL.Repository;

public interface ICommissionRequestRepository : IRepository <CommissionRequest, int>
{
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllNotAcceptedAsync(
      int paginationCursor,
      int pageSize);
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
      int paginationCursor,
      int pageSize);
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllFromOneWorkerAsync(
      int paginationCursor,
      int pageSize,
      int WorkerID);
     Task<CursorPaginatedEnumerable<Commission, int>> Async(
      int paginationCursor,
      int pageSize,
      int WorkerID);
}

internal class CommissionRequestRepository : Repository<CommissionRequest, int>, ICommissionRequestRepository
{
    public CommissionRequestRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CursorPaginatedEnumerable<CommissionRequest, int>> GetAllNotAcceptedAsync(
      int paginationCursor,
      int pageSize, DateOnly completed = null)
    {
        IQueyable<CommissionRequest> query = _untrackedSet.OrderByDescending(e => e.CommissionId);

        query = query.Where(e => e.CompletedAt == null);

        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);

    }
    public async Task<CursorPaginatedEnumerable<CommissionRequest, int>> GetAllRequestsAsync(
      int paginationCursor,
      int pageSize)
    {
        IQueyable<CommissionRequest> query = _untrackedSet.OrderByDescending(e => e.CommissionId);
        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);

    }
    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllEarkyRequestsAsync(
      int paginationCursor,
      int pageSize)
    {
        IQueyable<Commission> query = _untrackedSet.OrderByDescending(e => e.CommissionId);
        checkDate = DateTime.Now() - timedelta(days = 30);
        query = query.Where(e => e.CreatedAt >= checkDate);
        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);

    }






}
