using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("unit_of_measure");

        builder.HasKey(u => u.UnitOfMeasureId);

        builder.Property(u => u.UnitOfMeasureId)
            .HasColumnName("unit_of_measure_id");

        builder.Property(u => u.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(u => u.UnitOfMeasureCode)
            .HasColumnName("unit_of_measure_code")
            .HasMaxLength(100);

        builder.Property(u => u.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(u => u.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(u => u.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(u => u.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(u => u.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(u => u.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
