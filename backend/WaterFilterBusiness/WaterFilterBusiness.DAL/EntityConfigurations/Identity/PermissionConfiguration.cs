using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Identity;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(45);

        // Seeding
        IEnumerable<Permission> permissions = Enum.GetValues<Common.Enums.Permission>()
                                                  .Select(e => new Permission
                                                  {
                                                      Id = (int)e,
                                                      Name = e.ToString()
                                                  });

        builder.HasData(permissions);
    }
}
