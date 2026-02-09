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

        // Foreign key relationships
        builder.HasOne(cf => cf.Client)
            .WithMany()
            .HasForeignKey(cf => cf.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cf => cf.CreatedByAccount)
            .WithMany()
            .HasForeignKey(cf => cf.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cf => cf.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(cf => cf.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

