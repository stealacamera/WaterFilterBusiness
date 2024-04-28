using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Customers;

internal class CustomerChangeConfiguration : IEntityTypeConfiguration<CustomerChange>
{
    public void Configure(EntityTypeBuilder<CustomerChange> builder)
    {
        builder.ToTable("CustomerChangesHistory");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CustomerId)
               .IsRequired();

        builder.HasOne<Customer>()
               .WithMany()
               .HasForeignKey(e => e.CustomerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.Property(e => e.ChangedAt)
               .IsRequired();
    }
}
