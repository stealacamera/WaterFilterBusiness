﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterFilterBusiness.DAL.Entities;

namespace WaterFilterBusiness.DAL.EntityConfigurations
{
    internal class SalesAgentMeetingConfiguration : IEntityTypeConfiguration<SalesAgentMeeting>
    {
        public void Configure(EntityTypeBuilder<SalesAgentMeeting> builder)
        {
            builder.ToTable("SalesAgentsMeetings");

            builder.HasKey(e => e.Id);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.PhoneOperatorId)
                   .IsRequired();

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(e => e.SalesAgentId)
                   .IsRequired();

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(e => e.CustomerId)
                   .IsRequired();

            builder.Property(e => e.DateTime)
                   .IsRequired();

            builder.Property(e => e.InitialNotes)
                   .HasColumnType("tinytext");

            builder.Property(e => e.IsSuccessful)
                   .IsRequired();
        }
    }
}
