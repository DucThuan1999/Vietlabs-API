using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SampleMatrixConfiguration : IEntityTypeConfiguration<SampleMatrix>
{
    public void Configure(EntityTypeBuilder<SampleMatrix> builder)
    {
        // Table name mapping
        builder.ToTable("sample_matrix");

        // Primary key
        builder.HasKey(sm => sm.SampleMatrixId);

        // Column mappings
        builder.Property(sm => sm.SampleMatrixId)
            .HasColumnName("sample_matrix_id");

        builder.Property(sm => sm.SampleMatrixCode)
            .HasColumnName("sample_matrix_code")
            .HasMaxLength(100);

        builder.Property(sm => sm.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(sm => sm.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(sm => sm.SampleMatrixGroupId)
            .HasColumnName("sample_matrix_group_id")
            .IsRequired();

        builder.Property(sm => sm.RegisteredMatrix)
            .HasColumnName("registered_matrix")
            .HasMaxLength(200);

        builder.Property(sm => sm.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(sm => sm.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(sm => sm.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(sm => sm.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(sm => sm.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(sm => sm.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(sm => sm.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Foreign key relationship
        builder.HasOne(sm => sm.SampleMatrixGroup)
            .WithMany(smg => smg.SampleMatrices)
            .HasForeignKey(sm => sm.SampleMatrixGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

