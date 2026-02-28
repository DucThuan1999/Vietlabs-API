using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationApprovalThresholdConfiguration : IEntityTypeConfiguration<QuotationApprovalThreshold>
{
    public void Configure(EntityTypeBuilder<QuotationApprovalThreshold> builder)
    {
        // Table name mapping
        builder.ToTable("quotation_approval_threshold");

        // Primary key
        builder.HasKey(t => t.QuotationApprovalThresholdId);

        // Precision cho phần trăm
        builder.Property(t => t.MinDiscountPercent)
            .HasPrecision(5, 2);
        
        builder.Property(t => t.MaxDiscountPercent)
            .HasPrecision(5, 2);
    }
}

