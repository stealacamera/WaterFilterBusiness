using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;

namespace WaterFilterBusiness.BLL.Services.Inventory.Items;

public interface IBaseInventoryItemsService
{
    Task<Result<InventoryTypeItem>> UpsertAsync(int toolId, int quantity);
    Task<Result<InventoryTypeItem>> DecreaseQuantityAsync(int tooldId, int decreaseBy);
    Task<OffsetPaginatedList<InventoryTypeItem>> GetAllAsync(int page, int pageSize, bool? orderByQuantity = null);
}