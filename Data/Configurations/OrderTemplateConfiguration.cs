using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderTemplateConfiguration : IEntityTypeConfiguration<OrderTemplate>
{
    public void Configure(EntityTypeBuilder<OrderTemplate> builder)
    {
        builder.ToTable("order_template");

        builder.HasKey(t => t.TemplateId);

        builder.Property(t => t.TemplateId)
            .HasColumnName("template_id");

        builder.Property(t => t.OrderSampleId)
            .HasColumnName("order_sample_id")
            .IsRequired();

        builder.Property(t => t.QuotationId)
            .HasColumnName("quotation_id");

        builder.Property(t => t.TemplateName)
            .HasColumnName("template_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by");

        builder.HasOne(t => t.OrderSample)
            .WithMany(os => os.OrderTemplates)
            .HasForeignKey(t => t.OrderSampleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Quotation)
            .WithMany()
            .HasForeignKey(t => t.QuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CreatedByAccount)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.OrderSampleId, t.TemplateName })
            .HasDatabaseName("i_x_order_template_sample_name");

        builder.HasIndex(t => t.QuotationId)
            .HasDatabaseName("i_x_order_template_quotation_id");
    }
}

