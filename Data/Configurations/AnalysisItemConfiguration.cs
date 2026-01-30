using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class AnalysisItemConfiguration : IEntityTypeConfiguration<AnalysisItem>
{
    public void Configure(EntityTypeBuilder<AnalysisItem> builder)
    {
        // Table name mapping
        builder.ToTable("analysis_item");

        // Primary key
        builder.HasKey(ai => ai.AnalysisItemId);

        // Column mappings
        builder.Property(ai => ai.AnalysisItemId)
            .HasColumnName("analysis_item_id");

        builder.Property(ai => ai.AnalysisItemCode)
            .HasColumnName("analysis_item_code")
            .HasMaxLength(255);

        builder.Property(ai => ai.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(ai => ai.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(ai => ai.Organization)
            .HasColumnName("organization")
            .HasMaxLength(500);

        // Foreign Keys
        builder.Property(ai => ai.EquipmentTypeId)
            .HasColumnName("equipment_type_id")
            .IsRequired();

        builder.Property(ai => ai.AnalysisGroupId)
            .HasColumnName("analysis_group_id")
            .IsRequired();

        builder.Property(ai => ai.SampleMatrixId)
            .HasColumnName("sample_matrix_id")
            .IsRequired();

        builder.Property(ai => ai.SampleMatrixGroupId)
            .HasColumnName("sample_matrix_group_id")
            .IsRequired();

        builder.Property(ai => ai.PublishedGroupCode)
            .HasColumnName("published_group_code")
            .HasMaxLength(255);

        builder.Property(ai => ai.Lod)
            .HasColumnName("lod")
            .HasColumnType("decimal(10,3)");

        builder.Property(ai => ai.Loq)
            .HasColumnName("loq")
            .HasColumnType("decimal(10,3)");

        builder.Property(ai => ai.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ai => ai.Unit)
            .HasColumnName("unit")
            .HasMaxLength(50);

        // Boolean flags
        builder.Property(ai => ai.Nd107)
            .HasColumnName("nd_107")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.Iso)
            .HasColumnName("iso")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.CucBvtv)
            .HasColumnName("cuc_bvtv")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.BoCongThuong)
            .HasColumnName("bo_cong_thuong")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.Nafi)
            .HasColumnName("nafi")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.CucChanNuoi)
            .HasColumnName("cuc_chan_nuoi")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ai => ai.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(ai => ai.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(ai => ai.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ai => ai.UpdatedAt)
            .HasColumnName("updated_at");

        // Navigation Properties & Foreign Key Relationships
        builder.HasOne(ai => ai.EquipmentType)
            .WithMany()
            .HasForeignKey(ai => ai.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.AnalysisGroup)
            .WithMany(ag => ag.AnalysisItems)
            .HasForeignKey(ai => ai.AnalysisGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.SampleMatrix)
            .WithMany()
            .HasForeignKey(ai => ai.SampleMatrixId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.SampleMatrixGroup)
            .WithMany()
            .HasForeignKey(ai => ai.SampleMatrixGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

