using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class EquipmentTypeConfiguration : IEntityTypeConfiguration<EquipmentType>
{
    public void Configure(EntityTypeBuilder<EquipmentType> builder)
    {
        // Table name mapping
        builder.ToTable("equipment_type");

        // Primary key
        builder.HasKey(e => e.EquipmentTypeId);

        // Column mappings
        builder.Property(e => e.EquipmentTypeId)
            .HasColumnName("equipment_type_id");

        builder.Property(e => e.EquipmentTypeCode)
            .HasColumnName("equipment_type_code")
            .HasMaxLength(100);

        builder.Property(e => e.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(e => e.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");
    }
}

