using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class ClientDebtConfiguration : IEntityTypeConfiguration<ClientDebt>
{
    public void Configure(EntityTypeBuilder<ClientDebt> builder)
    {
        // Table name mapping
        builder.ToTable("client_debt");

        // Primary key
        builder.HasKey(cd => cd.ClientDebtId);

        // Column mappings
        builder.Property(cd => cd.ClientDebtId)
            .HasColumnName("client_debt_id");

        builder.Property(cd => cd.ClientId)
            .HasColumnName("client_id")
            .IsRequired();

        builder.Property(cd => cd.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(100);

        builder.Property(cd => cd.TotalDebt)
            .HasColumnName("total_debt")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cd => cd.DebtTermDays)
            .HasColumnName("debt_term_days")
            .IsRequired();

        builder.Property(cd => cd.CreditLimit)
            .HasColumnName("credit_limit")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(cd => cd.ContractEffectiveDate)
            .HasColumnName("contract_effective_date");

        builder.Property(cd => cd.ContractEndDate)
            .HasColumnName("contract_end_date");

        builder.Property(cd => cd.Attachments)
            .HasColumnName("attachments")
            .HasMaxLength(2000);

        builder.Property(cd => cd.LastSyncedAt)
            .HasColumnName("last_synced_at");

        builder.Property(cd => cd.MisaReferenceId)
            .HasColumnName("misa_reference_id")
            .HasMaxLength(255);

        // Thông tin liên hệ công nợ
        builder.Property(cd => cd.DebtContactName)
            .HasColumnName("debt_contact_name")
            .HasMaxLength(255);

        builder.Property(cd => cd.DebtContactPhone)
            .HasColumnName("debt_contact_phone")
            .HasMaxLength(50);

        builder.Property(cd => cd.DebtContactEmail)
            .HasColumnName("debt_contact_email")
            .HasMaxLength(255);

        builder.Property(cd => cd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(cd => cd.UpdatedAt)
            .HasColumnName("updated_at");
    }
}

