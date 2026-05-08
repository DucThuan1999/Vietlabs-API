using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderTemplatePackageConfiguration : IEntityTypeConfiguration<OrderTemplatePackage>
{
    public void Configure(EntityTypeBuilder<OrderTemplatePackage> builder)
    {
        builder.ToTable("order_template_package");

        builder.HasKey(p => p.OrderTemplatePackageId);

        builder.Property(p => p.OrderTemplatePackageId)
            .HasColumnName("order_template_package_id");

        builder.Property(p => p.TemplateId)
            .HasColumnName("template_id")
            .IsRequired();

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

        builder.HasOne(p => p.OrderTemplate)
            .WithMany(t => t.OrderTemplatePackages)
            .HasForeignKey(p => p.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.OrderTemplatePackageAnalysisItems)
            .WithOne(pai => pai.OrderTemplatePackage)
            .HasForeignKey(pai => pai.OrderTemplatePackageId)
            .OnDelete(DeleteBehavior.Cascade);

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
