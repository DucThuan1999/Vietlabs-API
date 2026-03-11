using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table name mapping
        builder.ToTable("employee");

        // Primary key
        builder.HasKey(e => e.EmployeeId);

        builder.Property(e => e.DepartmentId).HasColumnName("department_id");

        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(e => e.Department)
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

