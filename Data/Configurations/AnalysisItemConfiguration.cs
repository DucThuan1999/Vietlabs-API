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

        builder.Property(ai => ai.ShortName)
            .HasColumnName("short_name")
            .HasMaxLength(255);

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

        builder.Property(ai => ai.ReferenceMethodId)
            .HasColumnName("reference_method_id");

        builder.Property(ai => ai.StandardId)
            .HasColumnName("standard_id");

        builder.Property(ai => ai.UnitOfMeasureId)
            .HasColumnName("unit_of_measure_id");

        builder.Property(ai => ai.LaboratoryTechniqueId)
            .HasColumnName("laboratory_technique_id");

        builder.Property(ai => ai.PublishedGroupCode)
            .HasColumnName("published_group_code")
            .HasMaxLength(255);

        builder.Property(ai => ai.Lod)
            .HasColumnName("lod")
            .HasColumnType("decimal(10,3)");

        builder.Property(ai => ai.Loq)
            .HasColumnName("loq")
            .HasColumnType("decimal(10,3)");

        builder.Property(ai => ai.StandardValue)
            .HasColumnName("standard_value")
            .HasMaxLength(500);

        builder.Property(ai => ai.StandardQuantityText)
            .HasColumnName("standard_quantity_text")
            .HasMaxLength(500);

        builder.Property(ai => ai.StandardQuantityUnitOfMeasureId)
            .HasColumnName("standard_quantity_unit_of_measure_id");

        builder.Property(ai => ai.UnitPrice)
            .HasColumnName("unit_price")
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);

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

        builder.Property(ai => ai.UpdatedBy)
            .HasColumnName("updated_by");

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

        builder.HasOne(ai => ai.ReferenceMethod)
            .WithMany(rm => rm.AnalysisItems)
            .HasForeignKey(ai => ai.ReferenceMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.Standard)
            .WithMany(s => s.AnalysisItems)
            .HasForeignKey(ai => ai.StandardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.UnitOfMeasure)
            .WithMany(u => u.AnalysisItems)
            .HasForeignKey(ai => ai.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.StandardQuantityUnitOfMeasure)
            .WithMany()
            .HasForeignKey(ai => ai.StandardQuantityUnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.LaboratoryTechnique)
            .WithMany(lt => lt.AnalysisItems)
            .HasForeignKey(ai => ai.LaboratoryTechniqueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ai => ai.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(ai => ai.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

