using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Clients;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class ClientMeetingConfiguration : IEntityTypeConfiguration<ClientMeeting>
    {
        public void Configure(EntityTypeBuilder<ClientMeeting> builder)
        {
            builder.ToTable("SalesAgentsMeetings");

            builder.HasKey(e => e.Id);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.PhoneOperatorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany(e => e.ClientMeetings)
                   .HasForeignKey(e => e.SalesAgentId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne<Customer>()
                   .WithOne()
                   .HasForeignKey<ClientMeeting>(e => e.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.Property(e => e.ScheduledAt)
                   .IsRequired();

            builder.Property(e => e.InitialNotes)
                   .HasMaxLength(210);

            builder.Property(e => e.Afternotes)
                   .HasMaxLength(210);

            builder.HasOne<MeetingOutcome>()
                   .WithMany()
                   .HasForeignKey(e => e.MeetingOutcomeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
