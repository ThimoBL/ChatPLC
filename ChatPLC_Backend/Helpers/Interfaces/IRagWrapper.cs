namespace ChatPLC_Backend.Helpers;

public interface IRagWrapper
{
    Task<string> AskQuestion(string question);
    Task<bool> SendFileToRagModel(IFormFile file);
}