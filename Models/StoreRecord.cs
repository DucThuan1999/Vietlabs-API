namespace VietLab.Models;

public class StoreRecord
{
    public Guid StoreRecordId { get; set; }
    /// <summary>Module sở hữu file: Client, Quotation, PermissionRequest, ...</summary>
    public string ModuleCode { get; set; } = string.Empty;
    /// <summary>Id bản ghi trong bảng tương ứng ModuleCode.</summary>
    public Guid OwnerId { get; set; }
    public string? AttachmentName { get; set; }
    public string AttachmentPath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    public Account? UpdatedByAccount { get; set; }
}

