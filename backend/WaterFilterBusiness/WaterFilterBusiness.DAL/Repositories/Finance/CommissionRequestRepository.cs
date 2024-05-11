using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Finance;

public interface ICommissionRequestsRepository : ISimpleRepository<CommissionRequest, int>
{
    Task<OffsetPaginatedEnumerable<CommissionRequest>> GetAllAsync(
      int page,
      int pageSize,
      bool? filterByCompleted = false);

    Task<OffsetPaginatedEnumerable<CommissionRequest>> GetAllFromWorkerAsync(
      int page,
      int pageSize,
      int workerId,
      bool? filterByCompleted = false);
}

internal class CommissionRequestsRepository : SimpleRepository<CommissionRequest, int>, ICommissionRequestsRepository
{
    public CommissionRequestsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<OffsetPaginatedEnumerable<CommissionRequest>> GetAllAsync(
        int page,
        int pageSize,
        bool? filterByCompleted = false)
    {
        IQueryable<CommissionRequest> query = _untrackedSet.OrderBy(e => e.CommissionId);

        // If given value for filtering, gets completed requests
        // Otherwise gets uncompleted requests
        if (filterByCompleted.HasValue)
            query = query.Where(e => filterByCompleted.Value
                                     ? e.CompletedAt != null
                                     : e.CompletedAt == null);

        return OffsetPaginatedEnumerable<CommissionRequest>.CreateAsync(query, page, pageSize);
    }

    public Task<OffsetPaginatedEnumerable<CommissionRequest>> GetAllFromWorkerAsync(
        int page, 
        int pageSize, 
        int workerId,
        bool? filterByCompleted = false)
    {
        IQueryable<CommissionRequest> query = _untrackedSet.OrderByDescending(e => e.CommissionId);
        query = query.Where(e => e.Commission.WorkerId == workerId);

        // If given value for filtering, gets completed requests
        // Otherwise gets uncompleted requests
        if (filterByCompleted.HasValue)
            query = query.Where(e => filterByCompleted.Value
                                     ? e.CompletedAt != null
                                     : e.CompletedAt == null);

        return OffsetPaginatedEnumerable<CommissionRequest>.CreateAsync(query, page, pageSize);
    }
}
