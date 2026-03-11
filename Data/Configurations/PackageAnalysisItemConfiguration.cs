using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class PackageAnalysisItemConfiguration : IEntityTypeConfiguration<PackageAnalysisItem>
{
    public void Configure(EntityTypeBuilder<PackageAnalysisItem> builder)
    {
        // Table name mapping
        builder.ToTable("package_analysis_item");

        // Primary key
        builder.HasKey(pai => pai.PackageAnalysisItemId);

        // Column mappings
        builder.Property(pai => pai.PackageAnalysisItemId)
            .HasColumnName("package_analysis_item_id");

        builder.Property(pai => pai.PackageId)
            .HasColumnName("package_id")
            .IsRequired();

        builder.Property(pai => pai.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(pai => pai.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(pai => pai.IsRequired)
            .HasColumnName("is_required")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pai => pai.Notes)
            .HasColumnName("notes");

        builder.Property(pai => pai.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pai => pai.UpdatedAt)
            .HasColumnName("updated_at");
        builder.Property(pai => pai.UpdatedBy)
            .HasColumnName("updated_by");

        // Unique constraint: một package không thể có cùng analysis item 2 lần
        builder.HasIndex(pai => new { pai.PackageId, pai.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("UQ_package_analysis_item_package_item");

        // Navigation: PackageAnalysisItem -> Package
        builder.HasOne(pai => pai.Package)
            .WithMany(p => p.PackageAnalysisItems)
            .HasForeignKey(pai => pai.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation: PackageAnalysisItem -> AnalysisItem
        builder.HasOne(pai => pai.AnalysisItem)
            .WithMany()
            .HasForeignKey(pai => pai.AnalysisItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pai => pai.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(pai => pai.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
