using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderSamplePackageConfiguration : IEntityTypeConfiguration<OrderSamplePackage>
{
    public void Configure(EntityTypeBuilder<OrderSamplePackage> builder)
    {
        builder.ToTable("order_sample_package");

        builder.HasKey(p => p.OrderSamplePackageId);

        builder.Property(p => p.OrderSamplePackageId)
            .HasColumnName("order_sample_package_id");

        builder.Property(p => p.OrderSampleId)
            .HasColumnName("order_sample_id")
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

        builder.HasOne(p => p.OrderSample)
            .WithMany(os => os.OrderSamplePackages)
            .HasForeignKey(p => p.OrderSampleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.OrderSamplePackageAnalysisItems)
            .WithOne(pai => pai.OrderSamplePackage)
            .HasForeignKey(pai => pai.OrderSamplePackageId)
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
