using FluentResults;
using Microsoft.IdentityModel.Tokens;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Errors;
using WaterFilterBusiness.Common.Errors.Inventory;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Inventory.Items;

public interface IInventoryItemsService
{
    Task<Result<InventoryItem>> GetByIdAsync(int id);
    Task<InventoryItem> CreateAsync(InventoryItem_AddRequestModel item);
    Task<Result<InventoryItem>> UpdateAsync(int id, InventoryItem_PatchRequestModel item);
    Task<Result> RemoveAsync(int id);
    Task<OffsetPaginatedList<InventoryItem>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true);
}

internal class InventoryItemsService : Service, IInventoryItemsService
{
    public InventoryItemsService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<InventoryItem> CreateAsync(InventoryItem_AddRequestModel item)
    {
        var dbModel = await _workUnit.InventoryItemsRepository
                                     .AddAsync(new DAL.Entities.Inventory.InventoryItem
                                     {
                                         Name = item.Name,
                                         Price = item.Price
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<InventoryItem>> GetAllAsync(int page, int pageSize, bool excludeDeleted = true)
    {
        var result = await _workUnit.InventoryItemsRepository
                                    .GetAllAsync(page, pageSize, excludeDeleted);

        return new OffsetPaginatedList<InventoryItem>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Values = result.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<InventoryItem>> GetByIdAsync(int id)
    {
        var dbModel = await _workUnit.InventoryItemsRepository.GetByIdAsync(id);
        return dbModel == null ? InventoryItemErrors.NotFound : ConvertEntityToModel(dbModel);
    }

    public async Task<Result> RemoveAsync(int id)
    {
        var dbModel = await _workUnit.InventoryItemsRepository.GetByIdAsync(id);

        if (dbModel == null || dbModel.DeletedAt != null)
            return GeneralErrors.NotFoundError("Inventory item");

        dbModel.DeletedAt = DateTime.Now;
        await _workUnit.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<InventoryItem>> UpdateAsync(int id, InventoryItem_PatchRequestModel item)
    {
        var dbModel = await _workUnit.InventoryItemsRepository.GetByIdAsync(id);

        if (dbModel == null || dbModel.DeletedAt != null)
            return GeneralErrors.NotFoundError("Inventory item");

        if (!item.Name.IsNullOrEmpty())
            dbModel.Name = item.Name.Trim();

        if (item.Price.HasValue)
            dbModel.Price = item.Price.Value;

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    private InventoryItem ConvertEntityToModel(DAL.Entities.Inventory.InventoryItem entity)
    {
        return new InventoryItem
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price
        };
    }
}
