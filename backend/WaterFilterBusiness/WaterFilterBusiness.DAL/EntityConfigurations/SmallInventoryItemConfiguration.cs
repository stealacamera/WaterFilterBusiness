using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class SmallInventoryItemConfiguration : IEntityTypeConfiguration<SmallInventoryItem>
{
    public void Configure(EntityTypeBuilder<SmallInventoryItem> builder)
    {
        builder.ToTable("SmallInventoryItems");

        builder.HasKey(e => e.ToolId);

        builder.HasOne<InventoryItem>()
               .WithOne()
               .HasForeignKey<SmallInventoryItem>(e => e.ToolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity)
               .IsRequired();
    }
}
