using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationAnalysisGroupConfiguration : IEntityTypeConfiguration<QuotationAnalysisGroup>
{
    public void Configure(EntityTypeBuilder<QuotationAnalysisGroup> builder)
    {
        // Table name mapping
        builder.ToTable("quotation_analysis_group");

        // Primary key
        builder.HasKey(qag => qag.QuotationAnalysisGroupId);

        // Column mappings
        builder.Property(qag => qag.QuotationAnalysisGroupId)
            .HasColumnName("quotation_analysis_group_id");

        builder.Property(qag => qag.QuotationId)
            .HasColumnName("quotation_id")
            .IsRequired();

        builder.Property(qag => qag.AnalysisGroupId)
            .HasColumnName("analysis_group_id")
            .IsRequired();

        builder.Property(qag => qag.StepPrice)
            .HasColumnName("step_price")
            .HasColumnType("decimal(18,2)");

        builder.Property(qag => qag.GroupSalePrice)
            .HasColumnName("group_sale_price")
            .HasColumnType("decimal(18,2)");

        builder.Property(qag => qag.DiscountRate)
            .HasColumnName("discount_rate")
            .HasColumnType("decimal(5,2)");

        builder.Property(qag => qag.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(qag => qag.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(qag => qag.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(qag => qag.UpdatedBy)
            .HasColumnName("updated_by");

        // Unique constraint: một quotation không thể có cùng analysis group 2 lần
        builder.HasIndex(qag => new { qag.QuotationId, qag.AnalysisGroupId })
            .IsUnique()
            .HasDatabaseName("UQ_quotation_analysis_group_quotation_group");

        // Navigation: QuotationAnalysisGroup -> Quotation
        builder.HasOne(qag => qag.Quotation)
            .WithMany(q => q.QuotationAnalysisGroups)
            .HasForeignKey(qag => qag.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation: QuotationAnalysisGroup -> AnalysisGroup
        builder.HasOne(qag => qag.AnalysisGroup)
            .WithMany()
            .HasForeignKey(qag => qag.AnalysisGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(qag => qag.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(qag => qag.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

