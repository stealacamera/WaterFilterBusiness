using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class MeetingOutcomeConfiguration : IEntityTypeConfiguration<MeetingOutcome>
{
    public void Configure(EntityTypeBuilder<MeetingOutcome> builder)
    {
        builder.ToTable("MeetingOutcomes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .HasMaxLength(20)
               .IsRequired();

        // Seeding
        var data = Common.Enums.MeetingOutcome.List
                                              .Select(e => new MeetingOutcome
                                              {
                                                  Id = e.Value,
                                                  Name = e.Name
                                              });

        builder.HasData(data);
    }
}
