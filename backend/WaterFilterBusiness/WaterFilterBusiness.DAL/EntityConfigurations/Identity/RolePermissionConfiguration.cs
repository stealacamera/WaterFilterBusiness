using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Identity;

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
        #region Phone operator permissions
        builder.HasData(Create(Role.PhoneOperator, Permission.CreateCustomerCalls));
        builder.HasData(Create(Role.PhoneOperator, Permission.ReadScheduledCalls));
        builder.HasData(Create(Role.PhoneOperator, Permission.EditScheduledCalls));
        builder.HasData(Create(Role.PhoneOperator, Permission.ReadClientMeetingsForWorker));
        builder.HasData(Create(Role.PhoneOperator, Permission.CreateClientMeetings));
        builder.HasData(Create(Role.PhoneOperator, Permission.ReadCustomerDetails));
        builder.HasData(Create(Role.PhoneOperator, Permission.UpdateCustomers));
        builder.HasData(Create(Role.PhoneOperator, Permission.ReadSalesAgentSchedules));
        #endregion

        #region Marketing chief permissions
        builder.HasData(Create(Role.MarketingChief, Permission.ReadCustomerCalls));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadScheduledCalls));
        builder.HasData(Create(Role.MarketingChief, Permission.CreateScheduledCalls));
        builder.HasData(Create(Role.MarketingChief, Permission.DeleteScheduledCalls));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadClientDebts));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadSales));
        builder.HasData(Create(Role.MarketingChief, Permission.VerifySales));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadInventoryPurchases));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadClientMeetings));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadCustomers));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadCustomerDetails));
        builder.HasData(Create(Role.MarketingChief, Permission.UpdateCustomers));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadCustomerChangeHistories));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadSalesAgentSchedules));
        builder.HasData(Create(Role.MarketingChief, Permission.UpdateSalesAgentSchedules));
        builder.HasData(Create(Role.MarketingChief, Permission.DeleteSalesAgentSchedules));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadSalesAgentScheduleChangeHistories));
        builder.HasData(Create(Role.MarketingChief, Permission.ReadStatistics));
        builder.HasData(Create(Role.MarketingChief, Permission.CreateCustomers));
        #endregion

        #region Technician permissions
        builder.HasData(Create(Role.Technician, Permission.ReadTechnicianInventory));
        builder.HasData(Create(Role.Technician, Permission.ReadClientDebts));
        builder.HasData(Create(Role.Technician, Permission.EditClientDebts));
        builder.HasData(Create(Role.Technician, Permission.CreateTechinicianInventoryRequests));
        builder.HasData(Create(Role.Technician, Permission.ReadTechinicianInventoryRequests));
        builder.HasData(Create(Role.Technician, Permission.DecreaseTechnicianStock));
        #endregion

        #region Sales agent permissions
        builder.HasData(Create(Role.SalesAgent, Permission.CreateSales));
        builder.HasData(Create(Role.SalesAgent, Permission.ReadClientMeetingsForWorker));
        builder.HasData(Create(Role.SalesAgent, Permission.ConcludeClientMeetings));
        builder.HasData(Create(Role.SalesAgent, Permission.ReadCustomerDetails));
        builder.HasData(Create(Role.SalesAgent, Permission.CreateCustomers));
        builder.HasData(Create(Role.SalesAgent, Permission.ReadSalesAgentSchedules));
        builder.HasData(Create(Role.SalesAgent, Permission.CreateSalesAgentSchedules));
        builder.HasData(Create(Role.SalesAgent, Permission.ReadSalesAgentScheduleChangeHistories));
        #endregion

        #region Inventory manager permissions
        builder.HasData(Create(Role.InventoryManager, Permission.ManageBigInventory));
        builder.HasData(Create(Role.InventoryManager, Permission.ReadSmallInventory));
        builder.HasData(Create(Role.InventoryManager, Permission.ReadTechnicianInventory));
        builder.HasData(Create(Role.InventoryManager, Permission.ReadSmallInventoryRequests));
        builder.HasData(Create(Role.InventoryManager, Permission.ResolveSmallInventoryRequests));
        #endregion

        #region Operations chief permissions
        builder.HasData(Create(Role.OperationsChief, Permission.ReadSmallInventory));
        builder.HasData(Create(Role.OperationsChief, Permission.CreateSmallInventoryRequests));
        builder.HasData(Create(Role.OperationsChief, Permission.ReadSmallInventoryRequests));
        builder.HasData(Create(Role.OperationsChief, Permission.ReadTechinicianInventoryRequests));
        builder.HasData(Create(Role.OperationsChief, Permission.ResolveTechnicianInventoryRequests));
        builder.HasData(Create(Role.OperationsChief, Permission.ReadInventoryPurchases));
        #endregion

        #region Admin permissions
        builder.HasData(Create(Role.Admin, Permission.ManageUsers));
        builder.HasData(Create(Role.Admin, Permission.ReadCustomerChangeHistories));
        builder.HasData(Create(Role.Admin, Permission.ReadSalesAgentScheduleChangeHistories));
        #endregion
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
