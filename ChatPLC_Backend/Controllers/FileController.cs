using ChatPLC_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatPLC_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
        
        // Randomize filename
        var randomName = Path.GetRandomFileName();
        
        var trustedFileName = (randomName.Contains('.') ? randomName[..randomName.IndexOf('.')] : randomName)
                              + Path.GetExtension(file.FileName);
        
        await using var trustedStream = System.IO.File.Create(trustedFileName);
        
        await file.CopyToAsync(trustedStream);
        
        trustedStream.Close();

        // var success = await _ragService.SendFileToRagModel(file);

        var success = true;

        return success
            ? Ok("File sent to RAG model successfully.")
            : StatusCode(500, "Failed to send file to RAG model.");
    }
}