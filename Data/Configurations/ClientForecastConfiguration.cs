using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientForecastConfiguration : IEntityTypeConfiguration<ClientForecast>
{
    public void Configure(EntityTypeBuilder<ClientForecast> builder)
    {
        // Table name mapping
        builder.ToTable("client_forecast");

        // Primary key
        builder.HasKey(cf => cf.ClientForecastId);
    }
}

