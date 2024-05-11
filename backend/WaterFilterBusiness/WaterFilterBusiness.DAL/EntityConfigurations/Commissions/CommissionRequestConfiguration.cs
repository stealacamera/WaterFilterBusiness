using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Commissions;

internal class CommissionRequestConfiguration : IEntityTypeConfiguration<CommissionRequest>
{
    public void Configure(EntityTypeBuilder<CommissionRequest> builder)
    {
        builder.ToTable("CommissionRequests");

        builder.HasKey(x => x.CommissionId);

        builder.HasOne(e => e.Commission)
               .WithOne()
               .HasForeignKey<CommissionRequest>(e => e.CommissionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.CreatedAt)
               .IsRequired();
    }
}
