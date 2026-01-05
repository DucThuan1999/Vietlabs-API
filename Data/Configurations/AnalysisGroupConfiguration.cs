using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AnalysisGroupConfiguration : IEntityTypeConfiguration<AnalysisGroup>
{
    public void Configure(EntityTypeBuilder<AnalysisGroup> builder)
    {
        // Table name mapping
        builder.ToTable("analysis_group");

        // Primary key
        builder.HasKey(ag => ag.AnalysisGroupId);

        // Column mappings
        builder.Property(ag => ag.AnalysisGroupId)
            .HasColumnName("analysis_group_id");

        builder.Property(ag => ag.AnalysisGroupCode)
            .HasColumnName("analysis_group_code")
            .HasMaxLength(255);

        builder.Property(ag => ag.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(ag => ag.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(ag => ag.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(ag => ag.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(ag => ag.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ag => ag.UpdatedAt)
            .HasColumnName("updated_at");

        // Navigation: 1 AnalysisGroup có nhiều AnalysisItem
        builder.HasMany(ag => ag.AnalysisItems)
            .WithOne(ai => ai.AnalysisGroup)
            .HasForeignKey(ai => ai.AnalysisGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

