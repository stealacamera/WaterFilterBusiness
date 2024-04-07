using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;
using WaterFilterBusiness.DAL.Entities.Enums;
using WaterFilterBusiness.DAL.Entities.Inventory;

namespace WaterFilterBusiness.DAL.EntityConfigurations.Inventory
{
    internal class InventoryItemMovementConfiguration : IEntityTypeConfiguration<InventoryItemMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryItemMovement> builder)
        {
            builder.ToTable("InventoryItemMovements");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.OccurredAt)
                   .IsRequired();


            builder.Property(e => e.ToolId)
                   .IsRequired();

            builder.HasOne<InventoryItem>()
               .WithMany()
               .IsRequired()
               .HasForeignKey(e => e.ToolId)
               .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.GiverId)
                   .IsRequired();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.GiverId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.ReceiverId)
                   .IsRequired();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.ReceiverId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.FromInventoryId)
                   .IsRequired();

            builder.HasOne<InventoryType>()
                   .WithMany()
                   .HasForeignKey(e => e.FromInventoryId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(e => e.ToInventoryId)
                   .IsRequired();

            builder.HasOne<InventoryType>()
                   .WithMany()
                   .HasForeignKey(e => e.ToInventoryId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
