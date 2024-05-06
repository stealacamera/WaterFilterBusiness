using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Clients;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class ClientDebtConfiguration : IEntityTypeConfiguration<ClientDebt>
{
    public void Configure(EntityTypeBuilder<ClientDebt> builder)
    {
        builder.ToTable("ClientDebts");

        builder.HasKey(e => e.Id);


        builder.Property(e => e.SaleId)
               .IsRequired();

        builder.HasOne(e => e.Sale)
               .WithMany()
               .HasForeignKey(e => e.SaleId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);


        builder.Property(e => e.DeadlineAt)
               .IsRequired();

        builder.Property(e => e.Amount)
               .HasPrecision(10, 4)
               .IsRequired();
    }
}
