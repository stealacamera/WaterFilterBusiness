using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.SalesAgentSchedules;

internal class SalesAgentScheduleConfiguration : IEntityTypeConfiguration<SalesAgentSchedule>
{
    public void Configure(EntityTypeBuilder<SalesAgentSchedule> builder)
    {
        builder.ToTable("SalesAgentsSchedules");

        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.SalesAgent)
               .WithMany()
               .HasForeignKey(e => e.SalesAgentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.BeginHour)
               .IsRequired();

        builder.Property(e => e.EndHour)
               .IsRequired();


        builder.Property(e => e.DayOfWeekId)
               .IsRequired();

        builder.HasOne<Entities.Enums.DayOfWeek>()
               .WithMany()
               .HasForeignKey(e => e.DayOfWeekId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();


        builder.HasIndex(e => new { e.DayOfWeekId, e.BeginHour, e.SalesAgentId })
               .IsUnique();
    }
}
