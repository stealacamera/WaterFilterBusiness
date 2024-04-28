using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Clients;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(e => e.MeetingId);
            
            builder.HasOne(e => e.Meeting)
                   .WithOne()
                   .HasForeignKey<Sale>(e => e.MeetingId)
                   .IsRequired();


            builder.HasOne<PaymentType>()
                   .WithMany()
                   .HasForeignKey(e => e.PaymentTypeId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.PaymentTypeId)
                   .IsRequired();


            builder.Property(e => e.UpfrontPaymentAmount)
                   .IsRequired()
                   .HasPrecision(10, 4);

            builder.Property(e => e.TotalAmount)
                   .IsRequired()
                   .HasPrecision(10, 4);


            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.VerificationNote)
                   .HasMaxLength(210);
        }
    }
}
