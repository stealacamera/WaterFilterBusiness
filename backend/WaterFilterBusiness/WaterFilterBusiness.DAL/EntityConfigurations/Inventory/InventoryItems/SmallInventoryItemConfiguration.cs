using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory.InventoryItems;

internal class SmallInventoryItemConfiguration : IEntityTypeConfiguration<SmallInventoryItem>
{
    public void Configure(EntityTypeBuilder<SmallInventoryItem> builder)
    {
        builder.ToTable("SmallInventoryItems");

        builder.HasKey(e => e.ToolId);

        builder.HasOne(e => e.Tool)
               .WithOne()
               .HasForeignKey<SmallInventoryItem>(e => e.ToolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity)
               .IsRequired();
    }
}
