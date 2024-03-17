using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(45);

        builder.Property(e => e.Price)
               .IsRequired()
               .HasPrecision(10, 4);
    }
}
