using System.Text.Json.Serialization;

namespace ChatPLC_Backend.Models;

public class AnthropicApiError
{
    [JsonPropertyName("type")] 
    public string Type { get; set; } = null!;

    [JsonPropertyName("error")] 
    public ErrorDetail Error { get; set; }
}

public class ErrorDetail
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}