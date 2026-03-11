namespace VietLab.Models;

public class EquipmentType
{
    public Guid EquipmentTypeId { get; set; }
    public string? EquipmentTypeCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Account? UpdatedByAccount { get; set; }
}

