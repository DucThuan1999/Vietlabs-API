using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class DepartmentAnalysisCapabilityDesignationConfiguration : IEntityTypeConfiguration<DepartmentAnalysisCapabilityDesignation>
{
    public void Configure(EntityTypeBuilder<DepartmentAnalysisCapabilityDesignation> builder)
    {
        builder.ToTable("department_analysis_capability_designation");

        builder.HasKey(dacd => dacd.DepartmentAnalysisCapabilityDesignationId);

        builder.Property(dacd => dacd.DepartmentAnalysisCapabilityDesignationId)
            .HasColumnName("department_analysis_capability_designation_id");

        builder.Property(dacd => dacd.DepartmentAnalysisCapabilityId)
            .HasColumnName("department_analysis_capability_id")
            .IsRequired();

        builder.Property(dacd => dacd.DesignationId)
            .HasColumnName("designation_id")
            .IsRequired();

        builder.Property(dacd => dacd.ExpiredDate)
            .HasColumnName("expired_date");

        builder.HasIndex(dacd => new { dacd.DepartmentAnalysisCapabilityId, dacd.DesignationId })
            .IsUnique()
            .HasDatabaseName("IX_department_analysis_capability_designation_unique");

        builder.HasOne(dacd => dacd.DepartmentAnalysisCapability)
            .WithMany(dac => dac.Designations)
            .HasForeignKey(dacd => dacd.DepartmentAnalysisCapabilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dacd => dacd.Designation)
            .WithMany(d => d.DepartmentAnalysisCapabilityDesignations)
            .HasForeignKey(dacd => dacd.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
