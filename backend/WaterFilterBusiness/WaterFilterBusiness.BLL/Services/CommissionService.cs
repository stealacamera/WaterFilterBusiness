using System;
using System.Threading.Tasks;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Entities;

namespace WaterFilterBusiness.BLL;

public interface ICommissionService
{
    Task<Commission> CreateCustomerAddedCommission(int id, CommissionType commissionType);
    Task<Commission> CreateMonthlyPaymentComission(int id, CommissionType commissionType);
    Task<Commission> CreateWaterFilterCommission(int id, CommissionType commissionType)
    Task<Commission> SaleAgentSaleCommissionAsync(int id, int contract_payment, int referal_count, CommissionType commissionType);
    Task<Commission> PhoneAgentSaleCommissionAsync(int id, int contract_payment, CommissionType commissionType);
    Task<Commission> SaleAgentMonthlyCommissionAsync(int id, int sales_number, CommissionType commissionType);
    Task<Commission> PhoneAgentMonthlyCommissionAsync(int id, int sales_number, CommissionType commissionType);
    Task<Result<Commission>> GetByIdAsync(int id);
    Task<CursorPaginatedList<Commission, int>> GetAllAsync(int paginationCursor, int pageSize);
    Task<CursorPaginatedList<Commission, int>> GetAllync(int paginationCursor, int pageSize);
    Task<CursorPaginatedList<Commission, int>>> Update(int CommissionId);
}
public class CommissionService : Service, ICommissionService
{
    public CommissionService(iWorkUnit workUnit) : base(workUnit)
    {
    }

    public async Task<Commission> CreateCustomerAddedCommission(int id, CommissionType commissionType)
    {
        var dbModel = await _workUnit.CommissionRepository.AddSync(new DAL.Entities.Commission
        {
            Amount = 0.5,
            CommissionTypeId = commissionType,
            Reason = "Customer added during meeting ",
            ReleasedAt = DateTime.Now
        });
        await _workUnit.SaveChangesAsync();
        return dbModel;
    }

    Task<Commission> CreateMonthlyPaymentComission(int id, CommissionType commissionType)
    {
        var dbModel = await _workUnit.CommissionRepository.AddSync(new DAL.Entities.Commission
        {
            Amount = 5,
            CommissionTypeId = commissionType,
            Reason = "Set up a monthly payment contract ",
            ReleasedAt = DateTime.Now
        });
        await _workUnit.SaveChangesAsync();
        return dbModel;
    }
    Task<Commission> CreateWaterFilterCommission(int id, CommissionType commissionType)
    {
        var dbModel = await _workUnit.CommissionRepository.AddSync(new DAL.Entities.Commission
        {
            Amount = 10,
            CommissionTypeId = commissionType,
            Reason = "Phone agent set up a installation",
            ReleasedAt = DateTime.Now
        });
        await _workUnit.SaveChangesAsync();
        return dbModel;
    }
    Task<Commission> SaleAgentSaleCommissionAsync(int id, int contract_payment, int referal_count, CommissionType commissionType)
    {
        var amount;
        var Entity = new DAL.Enities.Commission
        {
            CommissionTypeId = commissionType,
            Reason = "Sale Agent got an upfront payment "+contract_payment+ "with "+referal_count,
            ReleasedAt = DateTime.Now
        };
        switch (contract_payment)
        {
            case >= 295:
                amount = 25;
                break;
            case <= 294 and >= 200:
                amount = 20;
                break;
            case <= 199 and >= 100:
                amount = 15;
                break;
            case <= 99 and >= 50:
                amount = 10;
                break
        }
        if (referal_count !=10)
        {
            amount = amount - 5;
        }
        Entity.Amount = amount;
        await _workUnit.CommissionRepository.AddSync(Entity);
        await _workUnit.SaveChangesAsync();
        return Entity;
    }

    Task<Commission> PhoneAgentSaleCommissionAsync(int id, int contract_payment, CommissionType commissionType)
    {
        if (contract_payment > 50)
        {
            var dbModel = await _workUnit.CommissionRepository.AddSync(new DAL.Entitis.Commission
            {
                Amount = 5,
                CommissionTypeId = 3,
                Reason = "Phone agent contributed in getting an upfront payment of " + contract_payment,
                ReleasedAt = DateTime.Now
            });
            await _workUnit.SaveChangesAsync();
            return ConvertEntityToModel(dbModel)
        }
    }
    Task<Commission> SaleAgentMonthlyCommissionAsync(int id, int sales_number, CommissionType commissionType)
    {
        var amount;
        var Entity = new DAL.Enities.Commission
        {
            CommissionTypeId = commissionType,
            Reason = "Sale Agent got an upfront payment " + contract_payment + "with " + referal_count,
            ReleasedAt = DateTime.Now
        };
        switch (contract_payment)
        {
            case >= 3 and <5:
                amount = 140 * sales_number;
                break;
            case >=5 and < 7:
                amount = 150 * sales_number;
                break;
            case >=7:
                amount = 160*sales_number;
                break;
        }
        Entity.Amount = amount;
        await _workUnit.CommissionRepository.AddSync(Entity);
        await _workUnit.SaveChangesAsync();
        return Entity;
    }
    Task<Commission> PhoneAgentMonthlyCommissionAsync(int id, int sales_number, CommissionType commissionType)
    {
        var dbModel = await _workUnit.CommissionRepository.AddSync(new DAL.Entitis.Commission
        {
            Amount = 1.5 * sales_number,
            CommissionTypId = commissionType,
            Reason = "Phon agent secured during the month " + sales_number + " sales",
            ReleasedAt = DateTime.Now
        });
        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel)
    }

    Task<Result<Commission>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.CommissionRepository.GetByIdAsync(id);
        return new Commission
        {
            Id = dbModel.Id,
            Amount = dbModel.Amount,
            CommissionTypeId = dbModel.CommissionTypeId,
            Reason = dbModel.Reason,
            ApprovedAt = dbModel.ApprovedAt,
            ReleasedAt = dbModel.ReleasedAt
        };
    }
    Task<CursorPaginatedList<Commission, int>> GetAllAsync(int paginationCursor, int pageSize)
    {
        var result = await _workUnit.CommissionRepository
                                    .GetAllAsync(
                                        paginationCursor, pageSize);
        return new CursorPaginatedList<ClientMeeting, int> 
        { 
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }
    Task<CursorPaginatedList<Commission, int>> GetAllFromOneWorkerAsync(int paginationCursor, int pageSize, int workerID)
    {
        var result = await _workUnit.CommissionRepository
                                    .GetAllFromOneWorkerAsync(
                                        paginationCursor, pageSize, workerID);
        return new CursorPaginatedList<ClientMeeting, int>
        {
            Cursor = result.Cursor,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }
    Task<CursorPaginatedList<Commission, int>>> Update(int commissionID)
    {
        var val = await _workUnit.CommissionRepository.GetByID(commissionID);
        val.ReleasedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();
    }

}