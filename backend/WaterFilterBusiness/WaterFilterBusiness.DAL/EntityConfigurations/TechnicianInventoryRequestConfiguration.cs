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

            builder.Property(e => e.TechnicianId)
                   .IsRequired();

            builder.Property(e => e.Note)
                   .HasColumnType("text");
        }
    }
}
