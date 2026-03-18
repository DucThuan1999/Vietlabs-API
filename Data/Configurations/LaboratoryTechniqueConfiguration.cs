using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class LaboratoryTechniqueConfiguration : IEntityTypeConfiguration<LaboratoryTechnique>
{
    public void Configure(EntityTypeBuilder<LaboratoryTechnique> builder)
    {
        builder.ToTable("laboratory_technique");

        builder.HasKey(lt => lt.LaboratoryTechniqueId);

        builder.Property(lt => lt.LaboratoryTechniqueId)
            .HasColumnName("laboratory_technique_id");

        builder.Property(lt => lt.SequenceNumber)
            .HasColumnName("sequence_number");

        builder.Property(lt => lt.TechniqueCode)
            .HasColumnName("technique_code")
            .HasMaxLength(100);

        builder.Property(lt => lt.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(500);

        builder.Property(lt => lt.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(500);

        builder.Property(lt => lt.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Active");

        builder.Property(lt => lt.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(lt => lt.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(lt => lt.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(lt => lt.UpdatedBy)
            .HasColumnName("updated_by");

        builder.HasOne(lt => lt.UpdatedByAccount)
            .WithMany()
            .HasForeignKey(lt => lt.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
