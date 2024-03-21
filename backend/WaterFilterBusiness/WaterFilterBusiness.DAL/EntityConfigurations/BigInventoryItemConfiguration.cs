using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class BigInventoryItemConfiguration : IEntityTypeConfiguration<BigInventoryItem>
{
    public void Configure(EntityTypeBuilder<BigInventoryItem> builder)
    {
        builder.ToTable("BigInventoryItems");

        builder.HasKey(e => e.ToolId);

        builder.HasOne<InventoryItem>()
               .WithOne()
               .HasForeignKey<BigInventoryItem>(e => e.ToolId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Quantity)
               .IsRequired();
    }
}