using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Commissions;

internal class CommissionConfiguration : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.ToTable("Commissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount)
               .HasPrecision(10, 4)
               .IsRequired();


        builder.Property(e => e.CommissionTypeId)
               .IsRequired();

        builder.HasOne<CommissionType>()
               .WithMany()
               .HasForeignKey(e => e.CommissionTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();


        builder.Property(e => e.Reason)
               .HasMaxLength(210)
               .IsRequired();


        builder.Property(e => e.WorkerId)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.WorkerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
