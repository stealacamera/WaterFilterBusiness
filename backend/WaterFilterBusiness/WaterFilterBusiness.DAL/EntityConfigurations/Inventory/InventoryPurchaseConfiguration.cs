using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory;

internal class InventoryPurchaseConfiguration : IEntityTypeConfiguration<InventoryPurchase>
{
    public void Configure(EntityTypeBuilder<InventoryPurchase> builder)
    {
        builder.ToTable("InventoryPurchases");

        builder.HasKey(x => x.Id);

        builder.Property(e => e.Quantity)
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(e => e.Price)
               .IsRequired()
               .HasPrecision(10, 4);

        builder.HasOne<InventoryItem>()
               .WithMany()
               .IsRequired()
               .HasForeignKey(e => e.ToolId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
