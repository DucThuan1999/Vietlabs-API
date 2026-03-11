using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationItemConfiguration : IEntityTypeConfiguration<QuotationItem>
{
    public void Configure(EntityTypeBuilder<QuotationItem> builder)
    {
        // Table name mapping
        builder.ToTable("quotation_item");

        // Primary key
        builder.HasKey(qi => qi.QuotationItemId);

        // Column mappings
        builder.Property(qi => qi.QuotationItemId)
            .HasColumnName("quotation_item_id");

        builder.Property(qi => qi.QuotationId)
            .HasColumnName("quotation_id")
            .IsRequired();

        builder.Property(qi => qi.ItemType)
            .HasColumnName("item_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(qi => qi.AnalysisItemId)
            .HasColumnName("analysis_item_id");

        builder.Property(qi => qi.AnalysisGroupId)
            .HasColumnName("analysis_group_id");

        builder.Property(qi => qi.PackageId)
            .HasColumnName("package_id");

        builder.Property(qi => qi.IsStandalone)
            .HasColumnName("is_standalone");

        builder.Property(qi => qi.ItemCode)
            .HasColumnName("item_code")
            .HasMaxLength(255);

        builder.Property(qi => qi.ItemNameVi)
            .HasColumnName("item_name_vi")
            .HasMaxLength(500);

        builder.Property(qi => qi.ItemNameEn)
            .HasColumnName("item_name_en")
            .HasMaxLength(500);

        builder.Property(qi => qi.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        // Snapshot fields từ AnalysisItem
        builder.Property(qi => qi.SampleMatrixName)
            .HasColumnName("sample_matrix_name")
            .HasMaxLength(500);

        builder.Property(qi => qi.PublishedGroupCode)
            .HasColumnName("published_group_code")
            .HasMaxLength(255);

        builder.Property(qi => qi.Unit)
            .HasColumnName("unit")
            .HasMaxLength(50);

        builder.Property(qi => qi.Lod)
            .HasColumnName("lod")
            .HasMaxLength(50);

        builder.Property(qi => qi.Loq)
            .HasColumnName("loq")
            .HasMaxLength(50);

        builder.Property(qi => qi.Tat)
            .HasColumnName("tat")
            .HasMaxLength(100);

        // Thông tin giá và số lượng
        builder.Property(qi => qi.Quantity)
            .HasColumnName("quantity")
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(qi => qi.DefaultPrice)
            .HasColumnName("default_price")
            .HasColumnType("decimal(18,2)");

        builder.Property(qi => qi.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(qi => qi.DiscountPercent)
            .HasColumnName("discount_percent")
            .HasColumnType("decimal(5,2)");

        builder.Property(qi => qi.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(qi => qi.SubTotal)
            .HasColumnName("sub_total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(qi => qi.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(qi => qi.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(qi => qi.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(qi => qi.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(qi => qi.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(qi => qi.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(qi => qi.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

