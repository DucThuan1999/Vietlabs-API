using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientIndustryConfiguration : IEntityTypeConfiguration<ClientIndustry>
{
    public void Configure(EntityTypeBuilder<ClientIndustry> builder)
    {
        builder.ToTable("client_industry");

        builder.HasKey(c => c.ClientIndustryId);

        builder.Property(c => c.ClientIndustryId)
            .HasColumnName("client_industry_id");

        builder.Property(c => c.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(c => c.IndustryCode)
            .HasColumnName("industry_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(200);

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(c => c.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.CreatedBy).HasColumnName("created_by");
        builder.Property(c => c.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(c => c.CreatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.IndustryCode);
        builder.HasIndex(c => c.Status);
    }
}
