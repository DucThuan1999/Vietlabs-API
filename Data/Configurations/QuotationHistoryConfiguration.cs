using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationHistoryConfiguration : IEntityTypeConfiguration<QuotationHistory>
{
    public void Configure(EntityTypeBuilder<QuotationHistory> builder)
    {
        // Table name mapping
        builder.ToTable("quotation_history");

        // Primary key
        builder.HasKey(qh => qh.QuotationHistoryId);

        // Column mappings
        builder.Property(qh => qh.QuotationHistoryId)
            .HasColumnName("quotation_history_id");

        builder.Property(qh => qh.QuotationId)
            .HasColumnName("quotation_id")
            .IsRequired();

        builder.Property(qh => qh.ChangedDate)
            .HasColumnName("changed_date")
            .IsRequired();

        builder.Property(qh => qh.ChangeDescription)
            .HasColumnName("change_description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(qh => qh.ChangedByAccountId)
            .HasColumnName("changed_by_account_id")
            .IsRequired();

        builder.Property(qh => qh.ChangeType)
            .HasColumnName("change_type")
            .HasMaxLength(50);

        builder.Property(qh => qh.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("nvarchar(max)");

        builder.Property(qh => qh.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("nvarchar(max)");

        // Foreign key relationships
        builder.HasOne(qh => qh.Quotation)
            .WithMany(q => q.QuotationHistories)
            .HasForeignKey(qh => qh.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qh => qh.ChangedByAccount)
            .WithMany()
            .HasForeignKey(qh => qh.ChangedByAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for better query performance
        builder.HasIndex(qh => qh.QuotationId);
        builder.HasIndex(qh => qh.ChangedDate);
        builder.HasIndex(qh => qh.ChangedByAccountId);
    }
}

