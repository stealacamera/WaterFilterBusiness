using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class InventoryItemMovementConfiguration : IEntityTypeConfiguration<InventoryItemMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryItemMovement> builder)
        {
            builder.ToTable("InventoryItemMovements");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ToolId)
                   .IsRequired();

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.FromInventory)
                   .IsRequired();

            builder.Property(e => e.ToInventory)
                   .IsRequired();

            builder.Property(e => e.OccurredAt)
                   .IsRequired();
        }
    }
}
