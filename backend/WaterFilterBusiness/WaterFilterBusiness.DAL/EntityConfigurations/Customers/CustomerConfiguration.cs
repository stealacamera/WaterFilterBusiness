using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Customers;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
               .IsRequired()
               .HasMaxLength(55);

        builder.HasIndex(e => e.FullName)
               .IsUnique();


        builder.Property(e => e.PhoneNumber)
               .IsRequired()
               .HasMaxLength(30);

        builder.HasIndex(e => e.PhoneNumber)
               .IsUnique();


        builder.Property(e => e.Address)
               .HasMaxLength(70);

        builder.Property(e => e.Profession)
               .IsRequired()
               .HasMaxLength(65);

        builder.Property(e => e.City)
               .IsRequired()
               .HasMaxLength(70);
    }
}
