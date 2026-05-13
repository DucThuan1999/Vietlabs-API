namespace VietLab.Models;

public class AnalysisItem
{
    public Guid AnalysisItemId { get; set; }
    public string? AnalysisItemCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string? ShortName { get; set; }  // Tên viết tắt

    // Foreign Keys
    public Guid? EquipmentTypeId { get; set; }
    public Guid? AnalysisGroupId { get; set; }
    public Guid SampleMatrixId { get; set; }
    public Guid SampleMatrixGroupId { get; set; }
    public Guid? ReferenceMethodId { get; set; }  // Phương pháp tham chiếu (mapping nội bộ – quốc tế)
    public Guid? StandardId { get; set; }  // Quy chuẩn/tiêu chuẩn áp dụng
    public Guid? UnitOfMeasureId { get; set; }  // Đơn vị tính (ĐVT)
    public Guid? LaboratoryTechniqueId { get; set; }  // Kĩ thuật (Sắc Ký, Cổ Điển, ...)
    
    public string? PublishedGroupCode { get; set; }
    public decimal? Lod { get; set; }
    public decimal? Loq { get; set; }
    public string? StandardValue { get; set; }  // Giá trị tiêu chuẩn (text)
    /// <summary>Khối lượng tiêu chuẩn (nhập text).</summary>
    public string? StandardQuantityText { get; set; }
    /// <summary>Đơn vị tính cho khối lượng tiêu chuẩn (riêng với UnitOfMeasureId).</summary>
    public Guid? StandardQuantityUnitOfMeasureId { get; set; }
    public decimal? UnitPrice { get; set; } // Đơn giá

    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation Properties
    public EquipmentType? EquipmentType { get; set; }
    public AnalysisGroup? AnalysisGroup { get; set; }
    public SampleMatrix? SampleMatrix { get; set; }
    public SampleMatrixGroup? SampleMatrixGroup { get; set; }
    public ReferenceMethod? ReferenceMethod { get; set; }
    public Standard? Standard { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }
    public UnitOfMeasure? StandardQuantityUnitOfMeasure { get; set; }
    public LaboratoryTechnique? LaboratoryTechnique { get; set; }
    public Account? UpdatedByAccount { get; set; }
    public ICollection<AnalysisItemTat> AnalysisItemTats { get; set; } = new List<AnalysisItemTat>();
}

