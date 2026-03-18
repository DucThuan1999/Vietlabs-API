using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietLab.Models;

namespace VietLab.Data.Configurations;

public class MatrixActionConfiguration : IEntityTypeConfiguration<MatrixAction>
{
    public void Configure(EntityTypeBuilder<MatrixAction> builder)
    {
        builder.ToTable("matrix_action");
        builder.HasKey(x => x.MatrixActionId);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
