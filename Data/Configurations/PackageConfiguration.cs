using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        // Table name mapping
        builder.ToTable("package");

        // Primary key
        builder.HasKey(p => p.PackageId);

        // Column mappings
        builder.Property(p => p.PackageId)
            .HasColumnName("package_id");

        builder.Property(p => p.PackageCode)
            .HasColumnName("package_code")
            .HasMaxLength(50);

        builder.Property(p => p.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(p => p.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .HasColumnName("description");

        builder.Property(p => p.DefaultPrice)
            .HasColumnName("default_price")
            .HasPrecision(18, 2);

        builder.Property(p => p.PublishedGroupCode)
            .HasColumnName("published_group_code")
            .HasMaxLength(100);

        builder.Property(p => p.SampleMatrixId)
            .HasColumnName("sample_matrix_id");

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(p => p.Notes)
            .HasColumnName("notes");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("updated_by");

        // Navigation: 1 Package có nhiều PackageAnalysisItem
        builder.HasMany(p => p.PackageAnalysisItems)
            .WithOne(pai => pai.Package)
            .HasForeignKey(pai => pai.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation: Package - SampleMatrix (optional)
        builder.HasOne(p => p.SampleMatrix)
            .WithMany()
            .HasForeignKey(p => p.SampleMatrixId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(p => p.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

