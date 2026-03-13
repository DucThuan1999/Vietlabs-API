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

    private string GetStoragePath(Guid? clientId)
    {
        var folderName = clientId.HasValue ? clientId.Value.ToString() : "Unassigned";
        var uploadsPath = Path.Combine(_environment.ContentRootPath, "Uploads", folderName);
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }
        return uploadsPath;
    }

    public async Task<StoreRecord> CreateFile(Guid? clientId, string? attachmentName, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        var folderName = clientId.HasValue ? clientId.Value.ToString() : "Unassigned";
        var storagePath = GetStoragePath(clientId);
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(storagePath, fileName);
        var relativePath = Path.Combine("Uploads", folderName, fileName).Replace("\\", "/");

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Create database record
        var storeRecord = new StoreRecord
        {
            StoreRecordId = Guid.NewGuid(),
            ClientId = clientId,
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

        // Save new file
        var folderName = storeRecord.ClientId.HasValue ? storeRecord.ClientId.Value.ToString() : "Unassigned";
        var storagePath = GetStoragePath(storeRecord.ClientId);
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(storagePath, fileName);
        var relativePath = Path.Combine("Uploads", folderName, fileName).Replace("\\", "/");

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

    public List<StoreRecord> GetFolderInfo(Guid clientId)
    {
        return _context.StoreRecords
            .Where(sr => sr.ClientId == clientId)
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

    public void DeleteFolder(Guid clientId)
    {
        var storagePath = GetStoragePath(clientId);
        if (Directory.Exists(storagePath))
        {
            Directory.Delete(storagePath, true);
        }

        var storeRecords = _context.StoreRecords
            .Where(sr => sr.ClientId == clientId)
            .ToList();

        if (storeRecords.Any())
        {
            _context.StoreRecords.RemoveRange(storeRecords);
            _context.SaveChanges();
        }
    }
}

