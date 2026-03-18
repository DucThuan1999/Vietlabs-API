using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AccountModuleGrantConfiguration : IEntityTypeConfiguration<AccountModuleGrant>
{
    public void Configure(EntityTypeBuilder<AccountModuleGrant> builder)
    {
        builder.ToTable("account_module_grant");
        builder.HasKey(x => x.AccountModuleGrantId);
        builder.HasIndex(x => new { x.AccountId, x.SecurityModuleId, x.MatrixActionId }).IsUnique();

        builder.HasOne(x => x.Account)
            .WithMany(a => a.ModuleGrants)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SecurityModule)
            .WithMany(m => m.AccountGrants)
            .HasForeignKey(x => x.SecurityModuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MatrixAction)
            .WithMany(a => a.AccountGrants)
            .HasForeignKey(x => x.MatrixActionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
