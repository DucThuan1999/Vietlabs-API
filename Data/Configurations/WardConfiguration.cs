using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        // Table name mapping
        builder.ToTable("ward");

        // Primary key
        builder.HasKey(w => w.WardId);

        // Column mappings
        builder.Property(w => w.WardId)
            .HasColumnName("ward_id");

        builder.Property(w => w.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(w => w.Code)
            .HasColumnName("code")
            .HasMaxLength(50);

        builder.Property(w => w.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Type)
            .HasColumnName("type")
            .HasMaxLength(100);

        builder.Property(w => w.ProvinceId)
            .HasColumnName("province_id")
            .IsRequired();

        builder.Property(w => w.CountryId)
            .HasColumnName("country_id")
            .IsRequired();

        builder.Property(w => w.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(w => w.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        // Foreign key relationships
        builder.HasOne(w => w.Province)
            .WithMany(p => p.Wards)
            .HasForeignKey(w => w.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Country)
            .WithMany()
            .HasForeignKey(w => w.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(w => w.ProvinceId);
        builder.HasIndex(w => w.CountryId);
        builder.HasIndex(w => w.Code);
        builder.HasIndex(w => w.Name);
        builder.HasIndex(w => w.Status);
    }
}

