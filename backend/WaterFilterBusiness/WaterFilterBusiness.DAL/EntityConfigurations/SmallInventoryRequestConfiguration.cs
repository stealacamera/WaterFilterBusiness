using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class SmallInventoryRequestConfiguration : IEntityTypeConfiguration<SmallInventoryRequest>
    {
        public void Configure(EntityTypeBuilder<SmallInventoryRequest> builder)
        {
            builder.ToTable("SmallInventoryRequests");

            builder.HasKey(e => e.Id);
        }
    }
}
