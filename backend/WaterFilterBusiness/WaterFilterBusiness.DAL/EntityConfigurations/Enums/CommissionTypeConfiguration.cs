using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class CommissionTypeConfiguration : IEntityTypeConfiguration<CommissionType>
{
    public void Configure(EntityTypeBuilder<CommissionType> builder)
    {
        builder.ToTable("CommissionTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(35);

        // Seeding
        var data = Common.Enums.CommissionType.List
                                              .Select(e => new CommissionType
                                              {
                                                  Id = e.Value,
                                                  Name = e.Name
                                              });

        builder.HasData(data);
    }
}
