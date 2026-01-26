using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Table name mapping
        builder.ToTable("client");

        // Primary key
        builder.HasKey(c => c.ClientId);

        // Column mappings sẽ được tự động convert sang snake_case
        // Chỉ cần set những cột đặc biệt nếu cần
    }
}

