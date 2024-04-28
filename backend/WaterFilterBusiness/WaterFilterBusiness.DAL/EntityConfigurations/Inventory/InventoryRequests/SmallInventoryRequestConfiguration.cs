using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory.InventoryRequests
{
    internal class SmallInventoryRequestConfiguration : IEntityTypeConfiguration<SmallInventoryRequest>
    {
        public void Configure(EntityTypeBuilder<SmallInventoryRequest> builder)
        {
            builder.ToTable("SmallInventoryRequests");

            builder.HasKey(e => e.InventoryRequestId);

            builder.HasOne(e => e.InventoryRequest)
                   .WithOne()
                   .IsRequired()
                   .HasForeignKey<SmallInventoryRequest>(e => e.InventoryRequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(e => e.RequesterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
