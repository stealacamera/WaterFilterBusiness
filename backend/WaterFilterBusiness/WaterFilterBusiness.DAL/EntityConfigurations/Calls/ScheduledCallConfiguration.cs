using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Calls;

internal class ScheduledCallConfiguration : IEntityTypeConfiguration<ScheduledCall>
{
    public void Configure(EntityTypeBuilder<ScheduledCall> builder)
    {
        builder.ToTable("ScheduledCalls");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ScheduledAt)
               .IsRequired();

        builder.HasOne<Customer>()
               .WithOne(e => e.ScheduledCall)
               .HasForeignKey<ScheduledCall>(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.PhoneAgentId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
