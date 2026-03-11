using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class DepartmentAnalysisCapabilityConfiguration : IEntityTypeConfiguration<DepartmentAnalysisCapability>
{
    public void Configure(EntityTypeBuilder<DepartmentAnalysisCapability> builder)
    {
        // Table name mapping
        builder.ToTable("department_analysis_capability");

        // Primary key
        builder.HasKey(dac => dac.DepartmentAnalysisCapabilityId);

        // Column mappings
        builder.Property(dac => dac.DepartmentAnalysisCapabilityId)
            .HasColumnName("department_analysis_capability_id");

        builder.Property(dac => dac.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dac => dac.BranchId)
            .HasColumnName("branch_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(dac => dac.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(dac => dac.Nd107)
            .HasColumnName("nd_107")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(dac => dac.Nd107ExpiredDate)
            .HasColumnName("nd_107_expired_date");

        builder.Property(dac => dac.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(dac => dac.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(dac => dac.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(dac => dac.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(dac => dac.UpdatedBy)
            .HasColumnName("updated_by");

        // Unique constraint on (department_id, branch_id, analysis_item_id)
        builder.HasIndex(dac => new { dac.DepartmentId, dac.BranchId, dac.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("IX_department_analysis_capability_unique");

        // Foreign Key Relationships
        builder.HasOne(dac => dac.Department)
            .WithMany()
            .HasForeignKey(dac => dac.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dac => dac.AnalysisItem)
            .WithMany()
            .HasForeignKey(dac => dac.AnalysisItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dac => dac.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(dac => dac.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

