using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientDebtConfiguration : IEntityTypeConfiguration<ClientDebt>
{
    public void Configure(EntityTypeBuilder<ClientDebt> builder)
    {
        // Table name mapping
        builder.ToTable("client_debt");

        // Primary key
        builder.HasKey(cd => cd.ClientDebtId);
    }
}

