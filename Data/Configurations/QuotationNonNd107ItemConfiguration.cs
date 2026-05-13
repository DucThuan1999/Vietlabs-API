using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationNonNd107ItemConfiguration : IEntityTypeConfiguration<QuotationNonNd107Item>
{
    public void Configure(EntityTypeBuilder<QuotationNonNd107Item> builder)
    {
        builder.ToTable("quotation_non_nd107_item");

        builder.HasKey(x => x.QuotationNonNd107ItemId);

        builder.Property(x => x.QuotationNonNd107ItemId).HasColumnName("quotation_non_nd107_item_id");
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.SourceType).HasColumnName("source_type").HasMaxLength(50).IsRequired();
        builder.Property(x => x.AnalysisItemId).HasColumnName("analysis_item_id");
        builder.Property(x => x.AnalysisGroupId).HasColumnName("analysis_group_id");
        builder.Property(x => x.PackageId).HasColumnName("package_id");

        builder.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(255);
        builder.Property(x => x.ItemNameVi).HasColumnName("item_name_vi").HasMaxLength(500);
        builder.Property(x => x.ItemNameEn).HasColumnName("item_name_en").HasMaxLength(500);
        builder.Property(x => x.SampleMatrixName).HasColumnName("sample_matrix_name").HasMaxLength(500);
        builder.Property(x => x.ReferenceMethodCode).HasColumnName("reference_method_code").HasMaxLength(255);
        builder.Property(x => x.ReferenceMethodNameVi).HasColumnName("reference_method_name_vi").HasMaxLength(500);
        builder.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(50);
        builder.Property(x => x.Lod).HasColumnName("lod").HasMaxLength(50);
        builder.Property(x => x.Loq).HasColumnName("loq").HasMaxLength(50);
        builder.Property(x => x.Tat).HasColumnName("tat").HasMaxLength(100);
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(x => x.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
