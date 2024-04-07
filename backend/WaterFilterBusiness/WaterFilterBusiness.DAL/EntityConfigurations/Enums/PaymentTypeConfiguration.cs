using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class PaymentTypeConfiguration : IEntityTypeConfiguration<PaymentType>
{
    public void Configure(EntityTypeBuilder<PaymentType> builder)
    {
        builder.ToTable("PaymentTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(25);

        // Seeding
        var data = Common.Enums.PaymentType.List
                                           .Select(e => new PaymentType
                                           {
                                               Id = e.Value,
                                               Name = e.Name
                                           });

        builder.HasData(data);
    }
}
