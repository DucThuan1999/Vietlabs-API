using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Models;

namespace VietLab.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<StoreRepository> _logger;

    public StoreRepository(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<StoreRepository> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    private string GetStoragePath(string moduleCode, Guid ownerId)
    {
        var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads", moduleCode, ownerId.ToString());
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }
        return uploadsPath;
    }

    public async Task<StoreRecord> CreateFile(string moduleCode, Guid ownerId, string? attachmentName, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required");
        }
        if (string.IsNullOrWhiteSpace(moduleCode))
        {
            throw new ArgumentException("ModuleCode is required");
        }

        var storagePath = GetStoragePath(moduleCode, ownerId);
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(storagePath, fileName);
        var relativePath = Path.Combine("Uploads", moduleCode, ownerId.ToString(), fileName).Replace("\\", "/");

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var storeRecord = new StoreRecord
        {
            StoreRecordId = Guid.NewGuid(),
            ModuleCode = moduleCode.Trim(),
            OwnerId = ownerId,
            AttachmentName = attachmentName ?? file.FileName,
            AttachmentPath = relativePath,
            FileName = file.FileName,
            FileSize = file.Length,
            ContentType = file.ContentType,
            CreatedDate = DateTime.UtcNow
        };

        _context.StoreRecords.Add(storeRecord);
        await _context.SaveChangesAsync();

        return storeRecord;
    }

    public async Task<StoreRecord> UpdateFile(Guid storeRecordId, string? attachmentName, IFormFile file)
    {
        var storeRecord = await _context.StoreRecords.FindAsync(storeRecordId);
        if (storeRecord == null)
        {
            throw new FileNotFoundException($"Store record with ID {storeRecordId} not found");
        }

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        // Delete old file
        var oldFilePath = Path.Combine(_environment.ContentRootPath, storeRecord.AttachmentPath);
        if (File.Exists(oldFilePath))
        {
            File.Delete(oldFilePath);
        }

        // Save new file (same module/owner as existing record)
        var storagePath = GetStoragePath(storeRecord.ModuleCode, storeRecord.OwnerId);
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(storagePath, fileName);
        var relativePath = Path.Combine("Uploads", storeRecord.ModuleCode, storeRecord.OwnerId.ToString(), fileName).Replace("\\", "/");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update database record
        storeRecord.AttachmentName = attachmentName ?? storeRecord.AttachmentName ?? file.FileName;
        storeRecord.AttachmentPath = relativePath;
        storeRecord.FileName = file.FileName;
        storeRecord.FileSize = file.Length;
        storeRecord.ContentType = file.ContentType;
        storeRecord.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return storeRecord;
    }

    public byte[] GetFileContent(Guid storeRecordId)
    {
        var storeRecord = _context.StoreRecords.Find(storeRecordId);
        if (storeRecord == null)
        {
            throw new FileNotFoundException($"Store record with ID {storeRecordId} not found");
        }

        var filePath = Path.Combine(_environment.ContentRootPath, storeRecord.AttachmentPath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found at path: {filePath}");
        }

        return File.ReadAllBytes(filePath);
    }

    public StoreRecord? GetFileInfo(Guid storeRecordId)
    {
        return _context.StoreRecords.Find(storeRecordId);
    }

    public List<StoreRecord> GetFolderInfo(string moduleCode, Guid ownerId)
    {
        return _context.StoreRecords
            .Where(sr => sr.ModuleCode == moduleCode && sr.OwnerId == ownerId)
            .OrderByDescending(sr => sr.CreatedDate)
            .ToList();
    }

    public void DeleteFile(string attachmentPath)
    {
        var filePath = Path.Combine(_environment.ContentRootPath, attachmentPath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var storeRecord = _context.StoreRecords
            .FirstOrDefault(sr => sr.AttachmentPath == attachmentPath);
        
        if (storeRecord != null)
        {
            _context.StoreRecords.Remove(storeRecord);
            _context.SaveChanges();
        }
    }

    public void DeleteFolder(string moduleCode, Guid ownerId)
    {
        var storagePath = GetStoragePath(moduleCode, ownerId);
        if (Directory.Exists(storagePath))
        {
            Directory.Delete(storagePath, true);
        }

        var storeRecords = _context.StoreRecords
            .Where(sr => sr.ModuleCode == moduleCode && sr.OwnerId == ownerId)
            .ToList();

        if (storeRecords.Any())
        {
            _context.StoreRecords.RemoveRange(storeRecords);
            _context.SaveChanges();
        }
    }
}

