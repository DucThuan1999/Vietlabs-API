using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        // Table name mapping
        builder.ToTable("contact");

        // Primary key
        builder.HasKey(c => c.ContactId);

        builder.HasAlternateKey(c => new { c.ContactId, c.ClientId })
            .HasName("AK_contact_contact_id_client_id");

        builder.HasOne(c => c.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

