using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ModuleApproverConfiguration : IEntityTypeConfiguration<ModuleApprover>
{
    public void Configure(EntityTypeBuilder<ModuleApprover> builder)
    {
        // Table name mapping
        builder.ToTable("module_approver");

        // Primary key
        builder.HasKey(m => m.ModuleApproverId);

        // Indexes để tối ưu truy vấn
        builder.HasIndex(m => new { m.ModuleCode, m.ApprovalLevel, m.PermissionId })
            .HasDatabaseName("IX_module_approver_module_level_permission");
        
        builder.HasIndex(m => m.ApproverEmployeeId)
            .HasDatabaseName("IX_module_approver_employee");
        
        builder.HasIndex(m => m.PermissionId)
            .HasDatabaseName("IX_module_approver_permission");

        // Quan hệ với Employee (người phê duyệt chỉ định)
        builder.HasOne(m => m.ApproverEmployee)
            .WithMany()
            .HasForeignKey(m => m.ApproverEmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Quan hệ với Permission
        builder.HasOne(m => m.Permission)
            .WithMany(p => p.ModuleApprovers)
            .HasForeignKey(m => m.PermissionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

