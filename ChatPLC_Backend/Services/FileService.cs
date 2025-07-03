using ChatPLC_Backend.Helpers;
using ChatPLC_Backend.Services.Interfaces;

namespace ChatPLC_Backend.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IRagWrapper _ragWrapper;

    public FileService(ILogger<FileService> logger, IRagWrapper ragWrapper)
    {
        _logger = logger;
        _ragWrapper = ragWrapper;
    }

    public async Task<bool> SendFileToRagModel(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Empty code received in {MethodName}", nameof(SendFileToRagModel));
            return false;
        }

        try
        {
            // Send the code to the RAG model
            var success = await _ragWrapper.SendFileToRagModel(code);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending file to RAG model in {MethodName}", nameof(SendFileToRagModel));
            return false;
        }
    }
}