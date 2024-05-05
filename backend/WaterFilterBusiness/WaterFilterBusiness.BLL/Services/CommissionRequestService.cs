using System;
using System.Threading.Tasks;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Entities;

namespace WaterFilterBusiness.BLL;

public interface ICommissionRequestService
{
    Task<Result<Commission>> GetByIdAsync(int id);
    Task<CursorPaginatedList<Commission, int>> GetAllAsync(int paginationCursor, int pageSize,DateTime CompletedAt);
}
public class CommissionService : Service, ICommissionService
    {
        public CommissionService(iWorkUnit workUnit) : base(workUnit)
        {
        }
    Task<Result<Commission>> GetByIdAsync(int id)
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
    Task<CursorPaginatedList<Commission, int>> GetAllAsync(int paginationCursor, int pageSize, DateTime CompletedAt)
    {

        var result = await _workUnit.CommissionRequestRepository
                                .GetAllAsync(
                                    paginationCursor, pageSize,
                                    CompletedAt);

        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }
}
