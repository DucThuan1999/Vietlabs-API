using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderSampleAnalysisGroupConfiguration : IEntityTypeConfiguration<OrderSampleAnalysisGroup>
{
    public void Configure(EntityTypeBuilder<OrderSampleAnalysisGroup> builder)
    {
        builder.ToTable("order_sample_analysis_group");

        builder.HasKey(qag => qag.OrderSampleAnalysisGroupId);

        builder.Property(qag => qag.OrderSampleAnalysisGroupId)
            .HasColumnName("order_sample_analysis_group_id");

        builder.Property(qag => qag.OrderSampleId)
            .HasColumnName("order_sample_id")
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

        builder.HasIndex(qag => new { qag.OrderSampleId, qag.AnalysisGroupId })
            .IsUnique()
            .HasDatabaseName("UQ_order_sample_analysis_group_sample_group");

        builder.HasOne(qag => qag.OrderSample)
            .WithMany(os => os.OrderSampleAnalysisGroups)
            .HasForeignKey(qag => qag.OrderSampleId)
            .OnDelete(DeleteBehavior.Cascade);

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
