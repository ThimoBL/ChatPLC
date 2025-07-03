using ChatPLC_Backend.Models;
using ChatPLC_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatPLC_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IFileService _fileService;

    public FileController(ILogger<FileController> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Upload(CodeResponse response)
    {
        if (response == null || string.IsNullOrWhiteSpace(response.Code))
        {
            _logger.LogWarning("Empty code response received in {MethodName}", nameof(Upload));
            return BadRequest("Code response cannot be empty.");
        }

        var success = await _fileService.SendFileToRagModel(response.Code);
        
        return success
            ? Ok("File sent to RAG model successfully.")
            : StatusCode(500, "Failed to send file to RAG model.");
    }
}