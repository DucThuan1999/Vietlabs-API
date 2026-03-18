using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietLab.Models;
using VietLab.Repositories;

namespace VietLab.Controllers;

[Authorize]
[ApiController]
[Route("odata/[controller]")]
public class StoreRecordsController : ControllerBase
{
    private readonly IStoreRepository _storeRepo;

    public StoreRecordsController(IStoreRepository storeRepo)
    {
        _storeRepo = storeRepo;
    }

    [HttpPost("create-file")]
    public async Task<IActionResult> CreateFile(string moduleCode, Guid ownerId, string? attachmentName, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                return BadRequest("ModuleCode is required.");
            }

            var storeRecord = await _storeRepo.CreateFile(moduleCode.Trim(), ownerId, attachmentName, file);
            return Ok(storeRecord);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update-file")]
    public async Task<IActionResult> UpdateFile(Guid storeRecordId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var updated = await _storeRepo.UpdateFile(storeRecordId, null, file);
            return Ok(updated);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-file-content")]
    public IActionResult GetFileContent(Guid storeRecordId)
    {
        try
        {
            var fileBytes = _storeRepo.GetFileContent(storeRecordId);

            return File(fileBytes, "application/octet-stream");
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-file-info")]
    public IActionResult GetFileInfo(Guid storeRecordId)
    {
        try
        {
            var fileInfo = _storeRepo.GetFileInfo(storeRecordId);
            if (fileInfo == null)
            {
                return NotFound("File not found.");
            }
            return Ok(fileInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-folder-info")]
    public IActionResult GetFolderInfo(string moduleCode, Guid ownerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                return BadRequest("ModuleCode is required.");
            }
            var filesInfo = _storeRepo.GetFolderInfo(moduleCode.Trim(), ownerId);
            return Ok(filesInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete-file")]
    public IActionResult DeleteFile(string attachmentPath)
    {
        try
        {
            _storeRepo.DeleteFile(attachmentPath);
            return Ok("File deleted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("delete-folder")]
    public IActionResult DeleteFolder(string moduleCode, Guid ownerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(moduleCode))
            {
                return BadRequest("ModuleCode is required.");
            }
            _storeRepo.DeleteFolder(moduleCode.Trim(), ownerId);
            return Ok("Folder deleted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
