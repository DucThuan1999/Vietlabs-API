using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SecurityModuleActionConfiguration : IEntityTypeConfiguration<SecurityModuleAction>
{
    public void Configure(EntityTypeBuilder<SecurityModuleAction> builder)
    {
        builder.ToTable("security_module_matrix_action");
        builder.HasKey(x => new { x.SecurityModuleId, x.MatrixActionId });

        builder.HasOne(x => x.SecurityModule)
            .WithMany(m => m.ModuleActions)
            .HasForeignKey(x => x.SecurityModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MatrixAction)
            .WithMany(a => a.ModuleActions)
            .HasForeignKey(x => x.MatrixActionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
