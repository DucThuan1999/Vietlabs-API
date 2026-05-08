using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderSamplePackageAnalysisItemConfiguration : IEntityTypeConfiguration<OrderSamplePackageAnalysisItem>
{
    public void Configure(EntityTypeBuilder<OrderSamplePackageAnalysisItem> builder)
    {
        builder.ToTable("order_sample_package_analysis_item");

        builder.HasKey(pai => pai.OrderSamplePackageAnalysisItemId);

        builder.Property(pai => pai.OrderSamplePackageAnalysisItemId)
            .HasColumnName("order_sample_package_analysis_item_id");

        builder.Property(pai => pai.OrderSamplePackageId)
            .HasColumnName("order_sample_package_id")
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

        builder.HasIndex(pai => new { pai.OrderSamplePackageId, pai.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("UQ_order_sample_package_analysis_item_pkg_item");

        builder.HasOne(pai => pai.OrderSamplePackage)
            .WithMany(p => p.OrderSamplePackageAnalysisItems)
            .HasForeignKey(pai => pai.OrderSamplePackageId)
            .OnDelete(DeleteBehavior.Cascade);

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
