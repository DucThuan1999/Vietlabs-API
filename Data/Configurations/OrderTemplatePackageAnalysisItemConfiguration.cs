using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderTemplatePackageAnalysisItemConfiguration : IEntityTypeConfiguration<OrderTemplatePackageAnalysisItem>
{
    public void Configure(EntityTypeBuilder<OrderTemplatePackageAnalysisItem> builder)
    {
        builder.ToTable("order_template_package_analysis_item");

        builder.HasKey(pai => pai.OrderTemplatePackageAnalysisItemId);

        builder.Property(pai => pai.OrderTemplatePackageAnalysisItemId)
            .HasColumnName("order_template_package_analysis_item_id");

        builder.Property(pai => pai.OrderTemplatePackageId)
            .HasColumnName("order_template_package_id")
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

        builder.HasIndex(pai => new { pai.OrderTemplatePackageId, pai.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("UQ_order_template_package_analysis_item_pkg_item");

        builder.HasOne(pai => pai.OrderTemplatePackage)
            .WithMany(p => p.OrderTemplatePackageAnalysisItems)
            .HasForeignKey(pai => pai.OrderTemplatePackageId)
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
