using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationIssueInfoConfiguration : IEntityTypeConfiguration<QuotationIssueInfo>
{
    public void Configure(EntityTypeBuilder<QuotationIssueInfo> builder)
    {
        builder.ToTable("quotation_issue_info");

        builder.HasKey(t => t.QuotationIssueInfoId);
    }
}
