using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ReferenceMethodConfiguration : IEntityTypeConfiguration<ReferenceMethod>
{
    public void Configure(EntityTypeBuilder<ReferenceMethod> builder)
    {
        builder.ToTable("reference_method");

        builder.HasKey(r => r.ReferenceMethodId);

        builder.Property(r => r.ReferenceMethodId)
            .HasColumnName("reference_method_id");

        builder.Property(r => r.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(r => r.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(r => r.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(r => r.ReferenceMethodCode)
            .HasColumnName("reference_method_code")
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(r => r.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(r => r.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(r => r.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(r => r.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
