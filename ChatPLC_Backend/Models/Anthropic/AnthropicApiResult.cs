namespace ChatPLC_Backend.Models;

public class AnthropicApiResult
{
    public AnthropicApiMessage Message { get; set; } = null!;
    public AnthropicApiError Error { get; set; } = null!;
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public bool IsSuccess => Message != null;
    
    public static AnthropicApiResult Success(AnthropicApiMessage message) => new() { Message = message };
    public static AnthropicApiResult Failure(AnthropicApiError error) => new() { Error = error };
}