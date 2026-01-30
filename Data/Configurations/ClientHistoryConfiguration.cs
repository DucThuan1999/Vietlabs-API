using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientHistoryConfiguration : IEntityTypeConfiguration<ClientHistory>
{
    public void Configure(EntityTypeBuilder<ClientHistory> builder)
    {
        // Table name mapping
        builder.ToTable("client_history");

        // Primary key
        builder.HasKey(ch => ch.ClientHistoryId);

        // Column mappings
        builder.Property(ch => ch.ClientHistoryId)
            .HasColumnName("client_history_id");

        builder.Property(ch => ch.ClientId)
            .HasColumnName("client_id")
            .IsRequired();

        builder.Property(ch => ch.ChangedDate)
            .HasColumnName("changed_date")
            .IsRequired();

        builder.Property(ch => ch.ChangeDescription)
            .HasColumnName("change_description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(ch => ch.ChangedByAccountId)
            .HasColumnName("changed_by_account_id")
            .IsRequired();

        builder.Property(ch => ch.ChangeType)
            .HasColumnName("change_type")
            .HasMaxLength(50);

        // Foreign key relationships
        builder.HasOne(ch => ch.Client)
            .WithMany()
            .HasForeignKey(ch => ch.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ch => ch.ChangedByAccount)
            .WithMany()
            .HasForeignKey(ch => ch.ChangedByAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for better query performance
        builder.HasIndex(ch => ch.ClientId);
        builder.HasIndex(ch => ch.ChangedDate);
        builder.HasIndex(ch => ch.ChangedByAccountId);
    }
}

