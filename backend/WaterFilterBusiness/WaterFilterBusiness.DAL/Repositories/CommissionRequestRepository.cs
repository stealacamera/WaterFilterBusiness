using System;

namespace WaterFilterBusiness.DAL.Repository;

public interface ICommissionRequestRepository : IRepository <CommissionRequest, int>
{
    Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
      int paginationCursor,
      int pageSize,
      DateOnly startingDate = null);
}

internal class CommissionRequestRepository : Repository<CommissionRequest, int>, ICommissionRequestRepository
{
    public CommissionRequestRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllAsync(
      int paginationCursor,
      int pageSize, DateOnly Accepted = null)
    {
        IQueyable<Commission> query = _untrackedSet.OrderByDescending(e => e.workerID);

        query = query.Where(e => e.CompletedAt == null);

        return await CursorPaginatedEnumerable<Commission, int>.CreateFromStrongEntityAsync(query, paginationCursor, pageSize);

    }

}
