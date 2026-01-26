using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class PackageAnalysisGroupConfiguration : IEntityTypeConfiguration<PackageAnalysisGroup>
{
    public void Configure(EntityTypeBuilder<PackageAnalysisGroup> builder)
    {
        // Table name mapping
        builder.ToTable("package_analysis_group");

        // Primary key
        builder.HasKey(pag => pag.PackageAnalysisGroupId);

        // Column mappings
        builder.Property(pag => pag.PackageAnalysisGroupId)
            .HasColumnName("package_analysis_group_id");

        builder.Property(pag => pag.PackageId)
            .HasColumnName("package_id")
            .IsRequired();

        builder.Property(pag => pag.AnalysisGroupId)
            .HasColumnName("analysis_group_id")
            .IsRequired();

        builder.Property(pag => pag.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(pag => pag.IsRequired)
            .HasColumnName("is_required")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pag => pag.Notes)
            .HasColumnName("notes");

        builder.Property(pag => pag.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Unique constraint: một package không thể có cùng analysis group 2 lần
        builder.HasIndex(pag => new { pag.PackageId, pag.AnalysisGroupId })
            .IsUnique()
            .HasDatabaseName("UQ_package_analysis_group_package_group");

        // Navigation: PackageAnalysisGroup -> Package
        builder.HasOne(pag => pag.Package)
            .WithMany(p => p.PackageAnalysisGroups)
            .HasForeignKey(pag => pag.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation: PackageAnalysisGroup -> AnalysisGroup
        builder.HasOne(pag => pag.AnalysisGroup)
            .WithMany()
            .HasForeignKey(pag => pag.AnalysisGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

