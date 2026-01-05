namespace VietLab.Models;

public class SampleMatrixGroup
{
    public Guid SampleMatrixGroupId { get; set; }
    public string? SampleMatrixGroupCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation: 1 SampleMatrixGroup có nhiều SampleMatrix
    public ICollection<SampleMatrix> SampleMatrices { get; set; } = new List<SampleMatrix>();
}

