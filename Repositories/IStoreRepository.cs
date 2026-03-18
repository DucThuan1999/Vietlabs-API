using VietLab.Models;

namespace VietLab.Repositories;

public interface IStoreRepository
{
    Task<StoreRecord> CreateFile(string moduleCode, Guid ownerId, string? attachmentName, IFormFile file);
    Task<StoreRecord> UpdateFile(Guid storeRecordId, string? attachmentName, IFormFile file);
    byte[] GetFileContent(Guid storeRecordId);
    StoreRecord? GetFileInfo(Guid storeRecordId);
    List<StoreRecord> GetFolderInfo(string moduleCode, Guid ownerId);
    void DeleteFile(string attachmentPath);
    void DeleteFolder(string moduleCode, Guid ownerId);
}

