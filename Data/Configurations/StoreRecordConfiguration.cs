using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class StoreRecordConfiguration : IEntityTypeConfiguration<StoreRecord>
{
    public void Configure(EntityTypeBuilder<StoreRecord> builder)
    {
        // Table name mapping
        builder.ToTable("store_record");

        // Primary key
        builder.HasKey(sr => sr.StoreRecordId);

        // Column mappings
        builder.Property(sr => sr.StoreRecordId)
            .HasColumnName("store_record_id");

        builder.Property(sr => sr.ClientId)
            .HasColumnName("client_id");

        builder.Property(sr => sr.AttachmentName)
            .HasColumnName("attachment_name")
            .HasMaxLength(500);

        builder.Property(sr => sr.AttachmentPath)
            .HasColumnName("attachment_path")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(sr => sr.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(500);

        builder.Property(sr => sr.FileSize)
            .HasColumnName("file_size");

        builder.Property(sr => sr.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100);

        builder.Property(sr => sr.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(sr => sr.UpdatedDate)
            .HasColumnName("updated_date");

        builder.Property(sr => sr.UpdatedBy)
            .HasColumnName("updated_by");

        // Foreign key relationship (optional: ClientId nullable)
        builder.HasOne(sr => sr.Client)
            .WithMany()
            .HasForeignKey(sr => sr.ClientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sr => sr.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(sr => sr.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

