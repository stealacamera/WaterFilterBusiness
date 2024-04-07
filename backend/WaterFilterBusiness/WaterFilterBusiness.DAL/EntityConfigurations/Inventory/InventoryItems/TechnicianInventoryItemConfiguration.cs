using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory.InventoryItems;

internal class TechnicianInventoryItemConfiguration : IEntityTypeConfiguration<TechnicianInventoryItem>
{
    public void Configure(EntityTypeBuilder<TechnicianInventoryItem> builder)
    {
        builder.ToTable("TechnicianInventoryItems");

        builder.HasKey(e => new { e.TechnicianId, e.ToolId });

        builder.HasOne<InventoryItem>()
               .WithOne()
               .HasForeignKey<TechnicianInventoryItem>(e => e.ToolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
               .WithMany()
               .IsRequired()
               .HasForeignKey(e => e.TechnicianId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity)
               .IsRequired();
    }
}
