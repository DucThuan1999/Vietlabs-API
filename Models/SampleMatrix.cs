namespace VietLab.Models;

public class SampleMatrix
{
    public Guid SampleMatrixId { get; set; }
    public string? SampleMatrixCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public Guid SampleMatrixGroupId { get; set; } // Foreign key
    public string? RegisteredMatrix { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public SampleMatrixGroup? SampleMatrixGroup { get; set; }
}

