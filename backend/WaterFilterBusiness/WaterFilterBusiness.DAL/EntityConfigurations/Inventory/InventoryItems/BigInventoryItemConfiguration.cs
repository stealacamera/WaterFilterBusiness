using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory.InventoryItems;

internal class BigInventoryItemConfiguration : IEntityTypeConfiguration<BigInventoryItem>
{
    public void Configure(EntityTypeBuilder<BigInventoryItem> builder)
    {
        builder.ToTable("BigInventoryItems");

        builder.HasKey(e => e.ToolId);

        builder.HasOne(e => e.Tool)
               .WithOne()
               .HasForeignKey<BigInventoryItem>(e => e.ToolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity)
               .IsRequired();
    }
}