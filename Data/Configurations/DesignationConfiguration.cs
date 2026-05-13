using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
{
    public void Configure(EntityTypeBuilder<Designation> builder)
    {
        builder.ToTable("designation");

        builder.HasKey(d => d.DesignationId);

        builder.Property(d => d.DesignationId)
            .HasColumnName("designation_id");

        builder.Property(d => d.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(d => d.DesignationCode)
            .HasColumnName("designation_code")
            .HasMaxLength(100);

        builder.Property(d => d.SymbolCode)
            .HasColumnName("symbol_code")
            .HasMaxLength(20);

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(500);

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasMaxLength(4000);

        builder.Property(d => d.Note)
            .HasColumnName("note")
            .HasMaxLength(2000);

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(d => d.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(d => d.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(d => d.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
