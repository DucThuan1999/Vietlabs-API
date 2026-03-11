using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        // Table name mapping
        builder.ToTable("department");

        // Primary key
        builder.HasKey(d => d.DepartmentId);

        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");
        builder.Property(d => d.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(d => d.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(d => d.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

