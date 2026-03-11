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
    }
}

