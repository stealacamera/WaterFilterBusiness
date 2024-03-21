using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class SalesConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.MeetingId)
                   .IsRequired();

            builder.Property(e => e.IsVerified)
                   .IsRequired();

            builder.Property(e => e.Price)
                   .IsRequired();

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.AuthenticatedAt);

            builder.Property(e => e.AuthenticationNote)
                   .HasColumnType("tinytext");
        }
    }
}
