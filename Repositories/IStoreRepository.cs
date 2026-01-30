using VietLab.Models;

namespace VietLab.Repositories;

public interface IStoreRepository
{
    Task<StoreRecord> CreateFile(Guid clientId, string? attachmentName, IFormFile file);
    Task<StoreRecord> UpdateFile(Guid storeRecordId, string? attachmentName, IFormFile file);
    byte[] GetFileContent(Guid storeRecordId);
    StoreRecord? GetFileInfo(Guid storeRecordId);
    List<StoreRecord> GetFolderInfo(Guid clientId);
    void DeleteFile(string attachmentPath);
    void DeleteFolder(Guid clientId);
}

