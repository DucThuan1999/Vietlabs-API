using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        // Table name mapping
        builder.ToTable("country");

        // Primary key
        builder.HasKey(c => c.CountryId);

        // Column mappings
        builder.Property(c => c.CountryId)
            .HasColumnName("country_id");

        builder.Property(c => c.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.FullNameVi)
            .HasColumnName("full_name_vi")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.FullNameEn)
            .HasColumnName("full_name_en")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Alpha2)
            .HasColumnName("alpha_2")
            .HasMaxLength(2);

        builder.Property(c => c.Alpha3)
            .HasColumnName("alpha_3")
            .HasMaxLength(3);

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(c => c.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(c => c.Alpha2);
        builder.HasIndex(c => c.Alpha3);
        builder.HasIndex(c => c.NameEn);
        builder.HasIndex(c => c.Status);
    }
}

