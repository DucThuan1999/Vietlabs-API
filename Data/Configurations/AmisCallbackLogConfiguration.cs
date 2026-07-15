using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AmisCallbackLogConfiguration : IEntityTypeConfiguration<AmisCallbackLog>
{
    public void Configure(EntityTypeBuilder<AmisCallbackLog> builder)
    {
        builder.ToTable("amis_callback_log");

        builder.HasKey(x => x.AmisCallbackLogId);

        builder.Property(x => x.AmisCallbackLogId)
            .HasColumnName("amis_callback_log_id");

        builder.Property(x => x.Success)
            .HasColumnName("success")
            .IsRequired();

        builder.Property(x => x.ErrorCode)
            .HasColumnName("error_code")
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2000);

        builder.Property(x => x.Signature)
            .HasColumnName("signature")
            .HasMaxLength(256);

        builder.Property(x => x.DataType)
            .HasColumnName("data_type")
            .IsRequired();

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OrgCompanyCode)
            .HasColumnName("org_company_code")
            .HasMaxLength(200);

        builder.Property(x => x.AppId)
            .HasColumnName("app_id")
            .HasMaxLength(100);

        builder.Property(x => x.IsSignatureValid)
            .HasColumnName("is_signature_valid")
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .HasColumnName("received_at")
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(x => x.ProcessingError)
            .HasColumnName("processing_error")
            .HasMaxLength(2000);

        builder.HasIndex(x => x.ReceivedAt);
        builder.HasIndex(x => x.DataType);
        builder.HasIndex(x => x.IsSignatureValid);
    }
}
