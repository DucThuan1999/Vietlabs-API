using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class VatRateConfiguration : IEntityTypeConfiguration<VatRate>
{
    public void Configure(EntityTypeBuilder<VatRate> builder)
    {
        builder.ToTable("vat_rate");

        builder.HasKey(t => t.VatRateId);

        builder.Property(t => t.Percent)
            .HasPrecision(5, 2);
    }
}
