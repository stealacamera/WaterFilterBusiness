using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class SalesAgentMeetingConfiguration : IEntityTypeConfiguration<SalesAgentMeeting>
{
    public void Configure(EntityTypeBuilder<SalesAgentMeeting> builder)
    {
        builder.ToTable("SalesAgentsMeetings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DateTime)
               .IsRequired();

        builder.Property(e => e.InitialNotes)
               .HasColumnType("text");

        builder.Property(e => e.IsSuccessful)
               .IsRequired();

        // Configure foreign key relationships
        builder.HasOne(e => e.PhoneOperator)
               .WithMany()
               .HasForeignKey(e => e.PhoneOperatorId);

        builder.HasOne(e => e.SalesAgent)
               .WithMany()
               .HasForeignKey(e => e.SalesAgentId);

        builder.HasOne(e => e.Customer)
               .WithMany()
               .HasForeignKey(e => e.CustomerId);
    }
}
