namespace VietLab.Models;

public class EquipmentType
{
    public Guid EquipmentTypeId { get; set; }
    public string? EquipmentTypeCode { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string Status { get; set; } = "Active";
}

