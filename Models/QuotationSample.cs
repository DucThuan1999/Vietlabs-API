namespace VietLab.Models;

/// <summary>
/// Dòng mô tả mẫu và khối lượng mẫu (freetext) gắn với một báo giá.
/// </summary>
public class QuotationSample
{
    public Guid QuotationSampleId { get; set; }
    public Guid QuotationId { get; set; }

    /// <summary>Mẫu (freetext).</summary>
    public string? SampleName { get; set; }

    /// <summary>Khối lượng mẫu (freetext).</summary>
    public string? SampleVolume { get; set; }

    public int? DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Quotation? Quotation { get; set; }
}
