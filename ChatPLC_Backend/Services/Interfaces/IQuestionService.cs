namespace ChatPLC_Backend.Services.Interfaces;

public interface IQuestionService
{
    IAsyncEnumerable<string> AskQuestionToStream(string question);
    Task<string> AskQuestionToJson(string question);
    Task<bool> AskQuestion(string question);
}