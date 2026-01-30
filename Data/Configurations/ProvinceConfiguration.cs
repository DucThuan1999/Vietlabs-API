using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        // Table name mapping
        builder.ToTable("province");

        // Primary key
        builder.HasKey(p => p.ProvinceId);

        // Column mappings
        builder.Property(p => p.ProvinceId)
            .HasColumnName("province_id");

        builder.Property(p => p.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Type)
            .HasColumnName("type")
            .HasMaxLength(100);

        builder.Property(p => p.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(500);

        builder.Property(p => p.CountryId)
            .HasColumnName("country_id")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(p => p.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        // Foreign key relationship
        builder.HasOne(p => p.Country)
            .WithMany(c => c.Provinces)
            .HasForeignKey(p => p.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.CountryId);
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Status);
    }
}

