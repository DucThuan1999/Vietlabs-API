using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SampleMatrixGroupConfiguration : IEntityTypeConfiguration<SampleMatrixGroup>
{
    public void Configure(EntityTypeBuilder<SampleMatrixGroup> builder)
    {
        // Table name mapping
        builder.ToTable("sample_matrix_group");

        // Primary key
        builder.HasKey(smg => smg.SampleMatrixGroupId);

        // Column mappings
        builder.Property(smg => smg.SampleMatrixGroupId)
            .HasColumnName("sample_matrix_group_id");

        builder.Property(smg => smg.SampleMatrixGroupCode)
            .HasColumnName("sample_matrix_group_code")
            .HasMaxLength(100);

        builder.Property(smg => smg.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(smg => smg.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(smg => smg.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(smg => smg.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(smg => smg.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(smg => smg.UpdatedAt)
            .HasColumnName("updated_at");
    }
}

