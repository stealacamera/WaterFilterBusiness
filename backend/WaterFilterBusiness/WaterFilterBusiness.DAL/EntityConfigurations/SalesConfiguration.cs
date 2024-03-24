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

            builder.HasOne<SalesAgentMeeting>()
                   .WithOne()
                   .HasForeignKey<Sale>(e => e.MeetingId)
                   .IsRequired(false);

            builder.Property(e => e.IsVerified)
                   .IsRequired();

            builder.Property(e => e.Price)
                               .IsRequired()
                               .HasPrecision(10, 4);

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.AuthenticationNote)
                   .HasColumnType("tinytext");
        }
    }
}
