using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class InventoryRequestConfiguration : IEntityTypeConfiguration<InventoryRequest>
    {
        public void Configure(EntityTypeBuilder<InventoryRequest> builder)
        {
            builder.ToTable("InventoryRequests");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ToolId)
                   .IsRequired();

            builder.Property(e => e.Status)
                   .IsRequired();

            builder.Property(e => e.Quantity)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.ConcludedAt)
                   .IsRequired();

            builder.Property(e => e.RequestNote)
                   .HasColumnType("tinytext");

            builder.Property(e => e.ConclusionNote)
                   .HasColumnType("tinytext");
        }
    }
}
