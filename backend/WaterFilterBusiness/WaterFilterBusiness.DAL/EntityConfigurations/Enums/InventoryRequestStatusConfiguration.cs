using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class InventoryRequestStatusConfiguration : IEntityTypeConfiguration<InventoryRequestStatus>
{
    public void Configure(EntityTypeBuilder<InventoryRequestStatus> builder)
    {
        builder.ToTable("InventoryRequestStatuses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(15);

        // Seeding
        var data = Common.Enums
                         .InventoryRequestStatus.List
                         .Select(e => new InventoryRequestStatus
                         {
                            Id = e.Value,
                            Name = e.Name
                         });

        builder.HasData(data);
    }
}
