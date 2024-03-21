using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations;

internal class RolePermissionConfiguration : IEntityTypeConfiguration<Entities.RolePermission>
{
    public void Configure(EntityTypeBuilder<Entities.RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(e => new { e.RoleId, e.PermissionId });

        builder.HasOne<Entities.Role>()
               .WithMany()
               .HasForeignKey(e => e.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Entities.Permission>()
               .WithMany()
               .HasForeignKey(e => e.PermissionId)
               .OnDelete(DeleteBehavior.Cascade);

        // Seeding
        builder.HasData(Create(Role.Admin, Permission.ReadUsers));
        builder.HasData(Create(Role.Admin, Permission.CreateUser));
        builder.HasData(Create(Role.Admin, Permission.DeleteUser));

        builder.HasData(Create(Role.InventoryManager, Permission.ReadInventoryItems));
        builder.HasData(Create(Role.InventoryManager, Permission.CreateInventoryItem));
        builder.HasData(Create(Role.InventoryManager, Permission.DeleteInventoryItem));
        builder.HasData(Create(Role.InventoryManager, Permission.UpdateInventoryItem));
        builder.HasData(Create(Role.InventoryManager, Permission.CreateInventoryPurchase));
        builder.HasData(Create(Role.InventoryManager, Permission.UpdateInventoryRequest));

        builder.HasData(Create(Role.OperationsChief, Permission.ReadInventoryItems));
        builder.HasData(Create(Role.OperationsChief, Permission.CreateInventoryRequest));
        builder.HasData(Create(Role.OperationsChief, Permission.ReadSales));
        builder.HasData(Create(Role.OperationsChief, Permission.UpdateSale));

        builder.HasData(Create(Role.Technician, Permission.ReadInventoryItems));
        builder.HasData(Create(Role.Technician, Permission.CreateInventoryRequest));

        builder.HasData(Create(Role.SalesAgent, Permission.CreateCustomer));
        builder.HasData(Create(Role.SalesAgent, Permission.ReadCustomerMeetings));
        builder.HasData(Create(Role.SalesAgent, Permission.UpdateCustomerMeeting));
        builder.HasData(Create(Role.SalesAgent, Permission.CreateSale));

        builder.HasData(Create(Role.PhoneOperator, Permission.ReadCustomers));
        builder.HasData(Create(Role.PhoneOperator, Permission.UpdateCustomer));
        builder.HasData(Create(Role.PhoneOperator, Permission.CreateCustomerMeeting));
        builder.HasData(Create(Role.PhoneOperator, Permission.UpdateCustomerMeeting));
    }

    private static Entities.RolePermission Create(Role role, Permission permission)
    {
        return new Entities.RolePermission
        {
            RoleId = role.Value,
            PermissionId = (int)permission
        };
    }
}
