using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class TechnicianInventoryRequestConfiguration : IEntityTypeConfiguration<TechnicianInventoryRequest>
    {
        public void Configure(EntityTypeBuilder<TechnicianInventoryRequest> builder)
        {
            builder.ToTable("TechnicianInventoryRequests");

            builder.HasKey(e => e.InventoryRequestId);

            builder.HasOne(e => e.InventoryRequest)
                   .WithOne()
                   .HasForeignKey<InventoryRequest>(e => e.Id);

            builder.HasOne(e => e.Technician)
                   .WithMany()
                   .HasForeignKey(e => e.TechnicianId);
        }
    }
}
