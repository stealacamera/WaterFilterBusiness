using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities.Enums;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory.InventoryRequests
{
    internal class InventoryRequestConfiguration : IEntityTypeConfiguration<InventoryRequest>
    {
        public void Configure(EntityTypeBuilder<InventoryRequest> builder)
        {
            builder.ToTable("InventoryRequests");

            builder.HasKey(e => e.Id);

            builder.HasOne<InventoryItem>()
                   .WithMany()
                   .HasForeignKey(e => e.ToolId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.StatusId)
                   .IsRequired();

            builder.HasOne<InventoryRequestStatus>()
                   .WithMany()
                   .HasForeignKey(e => e.StatusId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.RequestNote)
                   .HasMaxLength(210);

            builder.Property(e => e.ConclusionNote)
                   .HasMaxLength(210);
        }
    }
}
