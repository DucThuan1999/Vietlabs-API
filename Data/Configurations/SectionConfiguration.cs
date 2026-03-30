using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        // Table name mapping
        builder.ToTable("section");

        // Primary key
        builder.HasKey(s => s.SectionId);
        builder.Property(s => s.SectionId).HasColumnName("section_id");
        builder.Property(s => s.SectionCode).HasColumnName("section_code");
        builder.Property(s => s.DepartmentId).HasColumnName("department_id");
        builder.Property(s => s.NameVi).HasColumnName("name_vi");
        builder.Property(s => s.NameEn).HasColumnName("name_en");
        builder.Property(s => s.Notes).HasColumnName("notes");
        builder.Property(s => s.Status).HasColumnName("status").HasDefaultValue("Active");

        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(s => s.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(s => s.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
