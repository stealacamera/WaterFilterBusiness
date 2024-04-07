using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.SalesAgentSchedules;

internal class SalesAgentScheduleChangeConfiguration : IEntityTypeConfiguration<SalesAgentScheduleChange>
{
    public void Configure(EntityTypeBuilder<SalesAgentScheduleChange> builder)
    {
        builder.ToTable("SalesAgentScheduleChanges_History");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ChangedAt)
               .IsRequired();

        builder.Property(e => e.ScheduleId)
               .IsRequired();

        builder.HasOne(e => e.Schedule)
               .WithMany()
               .HasForeignKey(e => e.ScheduleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Entities.Enums.DayOfWeek>()
               .WithMany()
               .HasForeignKey(e => e.OldDayOfWeekId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
