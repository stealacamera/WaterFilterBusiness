using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        
    }
}
