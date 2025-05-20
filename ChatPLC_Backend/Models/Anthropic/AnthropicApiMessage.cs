using System.Text.Json.Serialization;

namespace ChatPLC_Backend.Models;

public class AnthropicApiMessage
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("content")]
    public List<ContentItem> Content { get; set; } = null!;

    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; } = null!;

    [JsonPropertyName("stop_sequence")]
    public string StopSequence { get; set; } = null!;

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = null!;
}

public class ContentItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;
}

public class Usage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("cache_creation_input_tokens")]
    public int CacheCreationInputTokens { get; set; }

    [JsonPropertyName("cache_read_input_tokens")]
    public int CacheReadInputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }
}