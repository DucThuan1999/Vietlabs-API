using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class StandardConfiguration : IEntityTypeConfiguration<Standard>
{
    public void Configure(EntityTypeBuilder<Standard> builder)
    {
        builder.ToTable("standard");

        builder.HasKey(s => s.StandardId);

        builder.Property(s => s.StandardId)
            .HasColumnName("standard_id");

        builder.Property(s => s.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(s => s.StandardCode)
            .HasColumnName("standard_code")
            .HasMaxLength(100);

        builder.Property(s => s.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(s => s.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(s => s.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(s => s.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(s => s.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(s => s.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
