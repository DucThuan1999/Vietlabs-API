using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderHistoryConfiguration : IEntityTypeConfiguration<OrderHistory>
{
    public void Configure(EntityTypeBuilder<OrderHistory> builder)
    {
        // Table name mapping
        builder.ToTable("order_histories");

        // Primary key
        builder.HasKey(oh => oh.OrderHistoryId);

        // Column mappings
        builder.Property(oh => oh.OrderHistoryId)
            .HasColumnName("order_history_id");

        builder.Property(oh => oh.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(oh => oh.ActivityDate)
            .HasColumnName("activity_date")
            .IsRequired();

        builder.Property(oh => oh.Activity)
            .HasColumnName("activity")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(oh => oh.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(oh => oh.Status)
            .HasColumnName("status")
            .HasMaxLength(50);

        builder.Property(oh => oh.CreatedByAccountId)
            .HasColumnName("created_by_account_id")
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(oh => oh.Order)
            .WithMany(o => o.OrderHistories)
            .HasForeignKey(oh => oh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oh => oh.CreatedByAccount)
            .WithMany()
            .HasForeignKey(oh => oh.CreatedByAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for better query performance
        builder.HasIndex(oh => oh.OrderId);
        builder.HasIndex(oh => oh.ActivityDate);
        builder.HasIndex(oh => oh.CreatedByAccountId);
    }
}

