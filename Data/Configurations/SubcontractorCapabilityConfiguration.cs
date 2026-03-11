using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SubcontractorCapabilityConfiguration : IEntityTypeConfiguration<SubcontractorCapability>
{
    public void Configure(EntityTypeBuilder<SubcontractorCapability> builder)
    {
        builder.ToTable("subcontractor_capability");

        builder.HasKey(sc => sc.SubcontractorCapabilityId);

        builder.Property(sc => sc.SubcontractorCapabilityId)
            .HasColumnName("subcontractor_capability_id");

        builder.Property(sc => sc.SubcontractorId)
            .HasColumnName("subcontractor_id")
            .IsRequired();

        builder.Property(sc => sc.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(sc => sc.Nd107)
            .HasColumnName("nd_107")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(sc => sc.Nd107ExpiredDate)
            .HasColumnName("nd_107_expired_date");

        builder.Property(sc => sc.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(sc => sc.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(sc => sc.CreatedAt).HasColumnName("created_at");
        builder.Property(sc => sc.UpdatedAt).HasColumnName("updated_at");
        builder.Property(sc => sc.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(sc => sc.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(sc => sc.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(sc => new { sc.SubcontractorId, sc.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("IX_subcontractor_capability_subcontractor_analysis_item");

        builder.HasIndex(sc => sc.Status);

        builder.HasOne(sc => sc.Subcontractor)
            .WithMany(s => s.SubcontractorCapabilities)
            .HasForeignKey(sc => sc.SubcontractorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sc => sc.AnalysisItem)
            .WithMany()
            .HasForeignKey(sc => sc.AnalysisItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
