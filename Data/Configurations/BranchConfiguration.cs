using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        // Table name mapping
        builder.ToTable("branch");

        // Primary key
        builder.HasKey(b => b.BranchId);

        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");
        builder.Property(b => b.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(b => b.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(b => b.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

