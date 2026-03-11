using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SubcontractorCapabilityDesignationConfiguration : IEntityTypeConfiguration<SubcontractorCapabilityDesignation>
{
    public void Configure(EntityTypeBuilder<SubcontractorCapabilityDesignation> builder)
    {
        builder.ToTable("subcontractor_capability_designation");

        builder.HasKey(scd => scd.SubcontractorCapabilityDesignationId);

        builder.Property(scd => scd.SubcontractorCapabilityDesignationId)
            .HasColumnName("subcontractor_capability_designation_id");

        builder.Property(scd => scd.SubcontractorCapabilityId)
            .HasColumnName("subcontractor_capability_id")
            .IsRequired();

        builder.Property(scd => scd.DesignationId)
            .HasColumnName("designation_id")
            .IsRequired();

        builder.Property(scd => scd.ExpiredDate)
            .HasColumnName("expired_date");

        builder.HasIndex(scd => new { scd.SubcontractorCapabilityId, scd.DesignationId })
            .IsUnique()
            .HasDatabaseName("IX_subcontractor_capability_designation_unique");

        builder.HasOne(scd => scd.SubcontractorCapability)
            .WithMany(sc => sc.Designations)
            .HasForeignKey(scd => scd.SubcontractorCapabilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(scd => scd.Designation)
            .WithMany(d => d.SubcontractorCapabilityDesignations)
            .HasForeignKey(scd => scd.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
