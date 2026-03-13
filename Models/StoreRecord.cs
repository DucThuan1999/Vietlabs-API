namespace VietLab.Models;

public class StoreRecord
{
    public Guid StoreRecordId { get; set; }
    public Guid? ClientId { get; set; }
    public string? AttachmentName { get; set; }
    public string AttachmentPath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }  // AccountId người cập nhật

    // Navigation property
    public Client? Client { get; set; }
    public Account? UpdatedByAccount { get; set; }
}

