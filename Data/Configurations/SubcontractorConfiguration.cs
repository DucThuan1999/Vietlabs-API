using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class SubcontractorConfiguration : IEntityTypeConfiguration<Subcontractor>
{
    public void Configure(EntityTypeBuilder<Subcontractor> builder)
    {
        builder.ToTable("subcontractor");

        builder.HasKey(s => s.SubcontractorId);

        builder.Property(s => s.SubcontractorId)
            .HasColumnName("subcontractor_id");

        builder.Property(s => s.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.ContactPerson)
            .HasColumnName("contact_person")
            .HasMaxLength(200);

        builder.Property(s => s.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(s => s.Email)
            .HasColumnName("email")
            .HasMaxLength(200);

        builder.Property(s => s.Address)
            .HasColumnName("address")
            .HasMaxLength(500);

        builder.Property(s => s.DepartmentId)
            .HasColumnName("department_id");

        builder.HasOne(s => s.Department)
            .WithMany()
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(s => s.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(s => s.TaxCode)
            .HasColumnName("tax_code")
            .HasMaxLength(50);
        builder.Property(s => s.BankAccountNumber)
            .HasColumnName("bank_account_number")
            .HasMaxLength(100);
        builder.Property(s => s.BankAccountHolder)
            .HasColumnName("bank_account_holder")
            .HasMaxLength(200);
        builder.Property(s => s.BankName)
            .HasColumnName("bank_name")
            .HasMaxLength(200);
        builder.Property(s => s.ContractStatus)
            .HasColumnName("contract_status")
            .HasMaxLength(50);
        builder.Property(s => s.PaymentCycle)
            .HasColumnName("payment_cycle")
            .HasMaxLength(50);
        builder.Property(s => s.PaymentDays)
            .HasColumnName("payment_days");

        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedBy).HasColumnName("updated_by");

        builder.HasOne(s => s.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(s => s.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.Code);
        builder.HasIndex(s => s.Status);
    }
}
