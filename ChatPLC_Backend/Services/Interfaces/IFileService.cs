namespace ChatPLC_Backend.Services.Interfaces;

public interface IFileService
{
    Task<bool> SendFileToRagModel(string code);
}