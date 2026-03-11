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

        builder.HasOne(c => c.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(c => c.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

