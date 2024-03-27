using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class SalesAgentsScheduleConfiguration : IEntityTypeConfiguration<SalesAgentSchedule>
{
    public void Configure(EntityTypeBuilder<SalesAgentSchedule> builder)
    {
        builder.ToTable("SalesAgentsSchedules");

        builder.HasKey(e => new { e.SalesAgentId, e.DayOfWeek });

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.SalesAgentId)
               .IsRequired();

        builder.Property(e => e.BeginHour)
               .IsRequired();

        builder.Property(e => e.EndHour)
               .IsRequired();
    }
}
