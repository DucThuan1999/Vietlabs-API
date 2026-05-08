using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class OrderSampleConfiguration : IEntityTypeConfiguration<OrderSample>
{
    public void Configure(EntityTypeBuilder<OrderSample> builder)
    {
        builder.ToTable("order_sample");

        builder.HasKey(os => os.OrderSampleId);

        builder.Property(os => os.OrderSampleId)
            .HasColumnName("order_sample_id");

        builder.Property(os => os.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(os => os.QuotationId)
            .HasColumnName("quotation_id");

        builder.Property(os => os.SampleIdentifier)
            .HasColumnName("sample_identifier")
            .HasMaxLength(100);

        builder.Property(os => os.SampleCode)
            .HasColumnName("sample_code")
            .HasMaxLength(100);

        builder.Property(os => os.SampleMatrixId)
            .HasColumnName("sample_matrix_id")
            .IsRequired();

        builder.Property(os => os.SampleName)
            .HasColumnName("sample_name")
            .HasMaxLength(500);

        builder.Property(os => os.SampleWeight)
            .HasColumnName("sample_weight")
            .HasPrecision(18, 2);

        builder.Property(os => os.SampleTemperature)
            .HasColumnName("sample_temperature")
            .HasPrecision(18, 2);

        builder.Property(os => os.ResultTurnaroundTimeRequirement)
            .HasColumnName("result_turnaround_time_requirement")
            .HasMaxLength(200);

        builder.Property(os => os.FeePercentage)
            .HasColumnName("fee_percentage")
            .HasPrecision(5, 2);

        builder.Property(os => os.SampleConditionDescription)
            .HasColumnName("sample_condition_description")
            .HasMaxLength(2000);

        builder.Property(os => os.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(os => os.AnalysisItemCount)
            .HasColumnName("analysis_item_count");

        builder.Property(os => os.NtpAnalysisItemCount)
            .HasColumnName("ntp_analysis_item_count");

        builder.Property(os => os.Amount)
            .HasColumnName("amount")
            .HasPrecision(18, 2);

        builder.Property(os => os.SampleReceivedDate)
            .HasColumnName("sample_received_date");

        builder.HasOne(os => os.Order)
            .WithMany(o => o.OrderSamples)
            .HasForeignKey(os => os.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(os => os.SampleMatrix)
            .WithMany()
            .HasForeignKey(os => os.SampleMatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(os => os.Quotation)
            .WithMany()
            .HasForeignKey(os => os.QuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(os => os.OrderId)
            .HasDatabaseName("i_x_order_sample_order_id");

        builder.HasIndex(os => os.QuotationId)
            .HasDatabaseName("i_x_order_sample_quotation_id");

        builder.HasIndex(os => os.SampleMatrixId)
            .HasDatabaseName("i_x_order_sample_sample_matrix_id");
    }
}

