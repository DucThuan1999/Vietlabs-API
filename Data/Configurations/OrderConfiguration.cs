using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("order");

        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.OrderId)
            .HasColumnName("order_id");

        builder.Property(o => o.ClientId)
            .HasColumnName("client_id")
            .IsRequired();

        builder.Property(o => o.ContactId)
            .HasColumnName("contact_id");

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(o => o.ExpectedCompletionDate)
            .HasColumnName("expected_completion_date");

        builder.Property(o => o.DebtType)
            .HasColumnName("debt_type")
            .HasMaxLength(200);

        builder.Property(o => o.DebtStatus)
            .HasColumnName("debt_status")
            .HasMaxLength(200);

        builder.Property(o => o.OrderStatus)
            .HasColumnName("order_status")
            .HasMaxLength(100);

        builder.Property(o => o.TestingPurpose)
            .HasColumnName("testing_purpose");

        builder.Property(o => o.TestMethod)
            .HasColumnName("test_method");

        builder.Property(o => o.ResultTurnaroundTimeRequirement)
            .HasColumnName("result_turnaround_time_requirement");

        builder.Property(o => o.ResultDeliveryChannel)
            .HasColumnName("result_delivery_channel");

        builder.Property(o => o.Language)
            .HasColumnName("language");

        builder.Property(o => o.Technique)
            .HasColumnName("technique");

        builder.Property(o => o.ComparisonStandard)
            .HasColumnName("comparison_standard");

        builder.Property(o => o.Total)
            .HasColumnName("total");

        builder.Property(o => o.SubtotalBeforeVat)
            .HasColumnName("subtotal_before_vat");

        builder.Property(o => o.Vat)
            .HasColumnName("vat");

        builder.Property(o => o.PaymentAmount)
            .HasColumnName("payment_amount");

        builder.Property(o => o.SampleReceiptMethod)
            .HasColumnName("sample_receipt_method");

        builder.Property(o => o.LaboratorySampleRetention)
            .HasColumnName("laboratory_sample_retention");

        builder.Property(o => o.AdditionalInformation)
            .HasColumnName("additional_information");

        builder.Property(o => o.TestRequestConfirmation)
            .HasColumnName("test_request_confirmation")
            .HasMaxLength(500);

        builder.Property(o => o.MailDocumentConfirmation)
            .HasColumnName("mail_document_confirmation")
            .HasMaxLength(500);

        builder.Property(o => o.PaymentMethod)
            .HasColumnName("payment_method");

        builder.HasOne(o => o.Contact)
            .WithMany()
            .HasForeignKey(o => new { o.ContactId, o.ClientId })
            .HasPrincipalKey(c => new { c.ContactId, c.ClientId })
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_order_contact_contact_id_client_id");

        builder.HasOne(o => o.CreatedByAccount)
            .WithMany()
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.ClientId)
            .HasDatabaseName("i_x_order_client_id");

        builder.HasIndex(o => o.ContactId)
            .HasDatabaseName("i_x_order_contact_id");

        builder.HasIndex(o => o.CreatedBy)
            .HasDatabaseName("i_x_order_created_by");
    }
}
