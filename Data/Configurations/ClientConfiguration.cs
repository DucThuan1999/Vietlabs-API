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

        builder.HasOne(c => c.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Country>()
            .WithMany()
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Province>()
            .WithMany()
            .HasForeignKey(c => c.ProvinceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Ward>()
            .WithMany()
            .HasForeignKey(c => c.WardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CountryId).HasDatabaseName("IX_client_country_id");
        builder.HasIndex(c => c.ProvinceId).HasDatabaseName("IX_client_province_id");
        builder.HasIndex(c => c.WardId).HasDatabaseName("IX_client_ward_id");
    }
}

