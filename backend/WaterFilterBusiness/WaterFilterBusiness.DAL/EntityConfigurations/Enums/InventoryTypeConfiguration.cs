using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class InventoryTypeConfiguration : IEntityTypeConfiguration<InventoryType>
{
    public void Configure(EntityTypeBuilder<InventoryType> builder)
    {
        builder.ToTable("InventoryTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(25);

        // Seeding
        var data = Common.Enums.InventoryType.List
                                             .Select(e => new InventoryType
                                             {
                                                 Id = e.Value,
                                                 Name = e.Name
                                             });

        builder.HasData(data);
    }
}
