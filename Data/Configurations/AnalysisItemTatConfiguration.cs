using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AnalysisItemTatConfiguration : IEntityTypeConfiguration<AnalysisItemTat>
{
    public void Configure(EntityTypeBuilder<AnalysisItemTat> builder)
    {
        // Table name mapping
        builder.ToTable("analysis_item_tat");

        // Primary key
        builder.HasKey(tat => tat.AnalysisItemTatId);

        // Column mappings
        builder.Property(tat => tat.AnalysisItemTatId)
            .HasColumnName("analysis_item_tat_id");

        builder.Property(tat => tat.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(tat => tat.TatType)
            .HasColumnName("tat_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(tat => tat.TatValue)
            .HasColumnName("tat_value")
            .IsRequired();

        builder.Property(tat => tat.TatUnit)
            .HasColumnName("tat_unit")
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Days");

        builder.Property(tat => tat.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000);

        builder.Property(tat => tat.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(tat => tat.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(tat => tat.UpdatedBy)
            .HasColumnName("updated_by");

        // Foreign key relationship
        builder.HasOne(tat => tat.AnalysisItem)
            .WithMany(ai => ai.AnalysisItemTats)
            .HasForeignKey(tat => tat.AnalysisItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: Mỗi AnalysisItem chỉ có 1 TAT cho mỗi loại
        builder.HasIndex(tat => new { tat.AnalysisItemId, tat.TatType })
            .IsUnique()
            .HasDatabaseName("IX_analysis_item_tat_item_type");

        // Index for better query performance
        builder.HasIndex(tat => tat.AnalysisItemId);
        builder.HasIndex(tat => tat.TatType);

        builder.HasOne(tat => tat.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(tat => tat.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

