using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class DayOfWeekConfiguration : IEntityTypeConfiguration<Entities.Enums.DayOfWeek>
{
    public void Configure(EntityTypeBuilder<Entities.Enums.DayOfWeek> builder)
    {
        builder.ToTable("DaysOfWeek");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(10);

        // Seeding
        var data = Common.Enums.Weekday.List
                                         .Select(e => new Entities.Enums.DayOfWeek
                                         {
                                             Id = e.Value,
                                             Name = e.Name
                                         });

        builder.HasData(data);
    }
}
