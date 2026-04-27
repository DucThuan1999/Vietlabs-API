using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationSampleConfiguration : IEntityTypeConfiguration<QuotationSample>
{
    public void Configure(EntityTypeBuilder<QuotationSample> builder)
    {
        builder.ToTable("quotation_sample");

        builder.HasKey(qs => qs.QuotationSampleId);

        builder.Property(qs => qs.QuotationSampleId)
            .HasColumnName("quotation_sample_id");

        builder.Property(qs => qs.QuotationId)
            .HasColumnName("quotation_id")
            .IsRequired();

        builder.Property(qs => qs.SampleName)
            .HasColumnName("sample_name")
            .HasMaxLength(2000);

        builder.Property(qs => qs.SampleVolume)
            .HasColumnName("sample_volume")
            .HasMaxLength(2000);

        builder.Property(qs => qs.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(qs => qs.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(qs => qs.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(qs => qs.QuotationId)
            .HasDatabaseName("i_x_quotation_sample_quotation_id");

        builder.HasOne(qs => qs.Quotation)
            .WithMany(q => q.QuotationSamples)
            .HasForeignKey(qs => qs.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
