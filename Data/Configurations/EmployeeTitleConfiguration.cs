using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class EmployeeTitleConfiguration : IEntityTypeConfiguration<EmployeeTitle>
{
    public void Configure(EntityTypeBuilder<EmployeeTitle> builder)
    {
        builder.ToTable("employee_title");

        builder.HasKey(e => e.EmployeeTitleId);

        builder.Property(e => e.EmployeeTitleId)
            .HasColumnName("employee_title_id");

        builder.Property(e => e.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(e => e.TitleCode)
            .HasColumnName("title_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(e => e.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(e => e.CreatedByAccount)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.TitleCode);
        builder.HasIndex(e => e.Status);
    }
}
