using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Clients;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Calls;

internal class CustomerCallConfiguration : IEntityTypeConfiguration<CustomerCall>
{
    public void Configure(EntityTypeBuilder<CustomerCall> builder)
    {
        builder.ToTable("CustomerCallsHistory");

        builder.HasKey(e => e.Id);


        builder.Property(e => e.PhoneAgentId)
               .IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.PhoneAgentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);


        builder.Property(e => e.CustomerId)
               .IsRequired();

        builder.HasOne<Customer>()
               .WithMany(e => e.CallHistory)
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();


        builder.Property(e => e.OccuredAt)
               .IsRequired();


        builder.Property(e => e.OutcomeId)
               .IsRequired();

        builder.HasOne<CallOutcome>()
               .WithMany()
               .HasForeignKey(e => e.OutcomeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
