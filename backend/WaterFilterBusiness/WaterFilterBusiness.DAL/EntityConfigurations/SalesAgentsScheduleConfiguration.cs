using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class SalesAgentsScheduleConfiguration : IEntityTypeConfiguration<SalesAgentsSchedule>
    {
        public void Configure(EntityTypeBuilder<SalesAgentsSchedule> builder)
        {
            builder.ToTable("SalesAgentsSchedules");

            builder.HasKey(e => new { e.SalesAgentId, e.DayOfWeek });

            builder.Property(e => e.BeginHour)
                   .IsRequired();

            builder.Property(e => e.EndHour)
                   .IsRequired();
        }
    }
}
