using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AnalysisItemDesignationConfiguration : IEntityTypeConfiguration<AnalysisItemDesignation>
{
    public void Configure(EntityTypeBuilder<AnalysisItemDesignation> builder)
    {
        builder.ToTable("analysis_item_designation");

        builder.HasKey(aid => aid.AnalysisItemDesignationId);

        builder.Property(aid => aid.AnalysisItemDesignationId)
            .HasColumnName("analysis_item_designation_id");

        builder.Property(aid => aid.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(aid => aid.DesignationId)
            .HasColumnName("designation_id")
            .IsRequired();

        builder.Property(aid => aid.ExpiredDate)
            .HasColumnName("expired_date");

        builder.HasIndex(aid => new { aid.AnalysisItemId, aid.DesignationId })
            .IsUnique()
            .HasDatabaseName("IX_analysis_item_designation_unique");

        builder.HasOne(aid => aid.AnalysisItem)
            .WithMany()
            .HasForeignKey(aid => aid.AnalysisItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(aid => aid.Designation)
            .WithMany(d => d.AnalysisItemDesignations)
            .HasForeignKey(aid => aid.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

