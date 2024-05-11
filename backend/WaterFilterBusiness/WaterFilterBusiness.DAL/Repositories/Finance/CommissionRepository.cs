using WaterFilterBusiness.DAL.DAOs;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Repository;

namespace WaterFilterBusiness.DAL.Repositories.Finance;

public interface ICommissionsRepository : ISimpleRepository<Commission, int>
{
    Task<OffsetPaginatedEnumerable<Commission>> GetAllAsync(
       int page,
       int pageSize,
       bool? filterByApproval = false,
       bool? filterByReleased = false);
    Task<OffsetPaginatedEnumerable<Commission>> GetAllForWorkerAsync(
        int page,
        int pageSize,
        int workerId);
}

internal class CommissionsRepository : SimpleRepository<Commission, int>, ICommissionsRepository
{
    public CommissionsRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OffsetPaginatedEnumerable<Commission>> GetAllAsync(
       int page,
       int pageSize,
       bool? filterByApproval = false,
       bool? filterByReleased = false)
    {
        IQueryable<Commission> query = _untrackedSet.OrderByDescending(e => e.WorkerId);

        // If given value for filtering, gets approved commissions
        // Otherwise gets unapproved commissions
        if (filterByApproval.HasValue)
            query = query.Where(e => filterByApproval.HasValue
                                     ? e.ApprovedAt != null
                                     : e.ApprovedAt == null);

        // If given value for filtering, gets released commissions
        // Otherwise gets unreleased commissions
        if (filterByReleased.HasValue)
            query = query.Where(e => filterByReleased.HasValue
                                     ? e.ReleasedAt != null
                                     : e.ReleasedAt == null);

        return await OffsetPaginatedEnumerable<Commission>.CreateAsync(query, page, pageSize);
    }

    public async Task<OffsetPaginatedEnumerable<Commission>> GetAllForWorkerAsync(
         int page,
         int pageSize,
         int workerId)
    {
        IQueryable<Commission> query = _untrackedSet.Where(e => e.WorkerId == workerId);
        query = query.OrderBy(e => e.Id);

        return await OffsetPaginatedEnumerable<Commission>.CreateAsync(query, page, pageSize);
    }

}