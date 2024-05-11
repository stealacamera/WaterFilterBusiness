using System;
using System.Threading.Tasks;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Entities;

namespace WaterFilterBusiness.BLL;

public interface ICommissionRequestService
{
    Task<Result<CommissionRequest>> GetByIdAsync(int id);
    Task<CursorPaginatedList<CommissionRequest, int>> GetAllAsync(int paginationCursor, int pageSize,DateTime CompletedAt);
    Task<CursorPaginatedList<CommissionRequest, int>>> Update(int commissionID);
    public async Task<CursorPaginatedEnumerable<CommissionRequest, int>> GetAllEarkyRequestsAsync(
      int paginationCursor,
      int pageSize);
}
public class CommissionSRequestervice : Service, ICommissionService
    {
        public CommissionRequestService(iWorkUnit workUnit) : base(workUnit)
        {
        }
    Task<Result<CommissionRequest>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.CommissionRequestRepository.GetByIdAsync(id);
        return new CommisionRequest
        {
            Id = dbModel.Id,
            CommissionId = dbModel.CommissionId,
            CreatedAt = dbModel.CreatedAt,
            CompletedAt = dbModel.CompletedAt
        };
    }
    Task<CursorPaginatedList<CommissionRequest, int>> GetAllFromOneWorkerAsync(int paginationCursor, int pageSize, int id)
    {

        var result = await _workUnit.CommissionRequestRepository
                                .GetAllAsync(
                                    paginationCursor, pageSize,
                                    id);

        return new CursorPaginatedList<CommissionRequest, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }
    Task<CursorPaginatedList<Commission, int>>> Update(int commissionID)
    {
        var val = await _workUnit.CommissionRequestRepository.GetByID(commissionID);
        val.ReleasedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();
    }
    public async Task<CursorPaginatedEnumerable<Commission, int>> GetAllEarkyRequestsAsync(
     int paginationCursor,
     int pageSize)
    {
        var result = await _workUnit.CommissionRequestRepository
                                .GetAllEarkyRequestsAsync(
                                    paginationCursor, pageSize,
                                    );

        return new CursorPaginatedList<CommissionRequest, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

}
