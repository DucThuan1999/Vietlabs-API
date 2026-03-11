using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class EmployeeAnalysisCapabilityConfiguration : IEntityTypeConfiguration<EmployeeAnalysisCapability>
{
    public void Configure(EntityTypeBuilder<EmployeeAnalysisCapability> builder)
    {
        builder.ToTable("employee_analysis_capability");

        builder.HasKey(eac => eac.EmployeeAnalysisCapabilityId);

        builder.Property(eac => eac.EmployeeAnalysisCapabilityId)
            .HasColumnName("employee_analysis_capability_id");

        builder.Property(eac => eac.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(eac => eac.AnalysisItemId)
            .HasColumnName("analysis_item_id")
            .IsRequired();

        builder.Property(eac => eac.Status)
            .HasColumnName("status");

        builder.Property(eac => eac.Notes)
            .HasColumnName("notes");

        builder.Property(eac => eac.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(eac => eac.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(eac => eac.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasIndex(eac => new { eac.EmployeeId, eac.AnalysisItemId })
            .IsUnique()
            .HasDatabaseName("UQ_employee_analysis_capability_employee_item");

        builder.HasOne(eac => eac.Employee)
            .WithMany()
            .HasForeignKey(eac => eac.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(eac => eac.AnalysisItem)
            .WithMany()
            .HasForeignKey(eac => eac.AnalysisItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(eac => eac.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(eac => eac.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
