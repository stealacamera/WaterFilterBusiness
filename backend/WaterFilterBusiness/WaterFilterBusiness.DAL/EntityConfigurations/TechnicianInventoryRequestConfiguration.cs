﻿using Microsoft.EntityFrameworkCore;
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

            builder.HasOne<InventoryRequest>()
                   .WithOne()
                   .IsRequired()
                   .HasForeignKey<TechnicianInventoryRequest>(e => e.InventoryRequestId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.TechnicianId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
