using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Identity;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.Property(e => e.Id)
               .ValueGeneratedNever();

        builder.HasMany(e => e.Permissions)
               .WithMany();

        // Seeding
        foreach (var role in Common.Enums.Role.List)
            builder.HasData(new Role
            {
                Id = role.Value,
                ConcurrencyStamp = role.Value.ToString(),
                Name = role.Name,
                NormalizedName = role.Name.ToUpper()
            });
    }
}
