using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SecurityModuleConfiguration : IEntityTypeConfiguration<SecurityModule>
{
    public void Configure(EntityTypeBuilder<SecurityModule> builder)
    {
        builder.ToTable("security_module");
        builder.HasKey(x => x.SecurityModuleId);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
