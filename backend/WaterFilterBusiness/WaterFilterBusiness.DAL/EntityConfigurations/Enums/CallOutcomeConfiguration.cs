using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Enums;

internal class CallOutcomeConfiguration : IEntityTypeConfiguration<CallOutcome>
{
    public void Configure(EntityTypeBuilder<CallOutcome> builder)
    {
        builder.ToTable("CallOutcomes");

        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(20);

        // Seeding
        var data = Common.Enums.CallOutcome.List
                                           .Select(e => new CallOutcome
                                           {
                                               Id = e.Value,
                                               Name = e.Name
                                           });

        builder.HasData(data);
    }
}
