using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class RegistrationPermitLabelConfigConfiguration : IEntityTypeConfiguration<RegistrationPermitLabelConfig>
{
    public void Configure(EntityTypeBuilder<RegistrationPermitLabelConfig> builder)
    {
        builder.ToTable("registration_permit_label_config");

        builder.HasKey(t => t.RegistrationPermitLabelConfigId);

        builder.Property(t => t.RegistrationPermitLabelConfigId)
            .HasColumnName("registration_permit_label_config_id");

        builder.Property(t => t.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(t => t.UpdatedBy)
            .HasColumnName("updated_by");
    }
}
