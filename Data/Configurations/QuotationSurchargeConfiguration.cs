using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class QuotationSurchargeConfiguration : IEntityTypeConfiguration<QuotationSurcharge>
{
    public void Configure(EntityTypeBuilder<QuotationSurcharge> builder)
    {
        builder.ToTable("quotation_surcharge");

        builder.HasKey(x => x.QuotationSurchargeId);

        builder.Property(x => x.QuotationSurchargeId).HasColumnName("quotation_surcharge_id");
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.SurchargeType).HasColumnName("surcharge_type").HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.DisplayOrder).HasColumnName("display_order");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(x => x.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
