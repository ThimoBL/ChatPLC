namespace ChatPLC_Backend.Models;

public class AnthropicApiExample
{
    public required string Question { get; set; }
    public required string IdealOutput { get; set; }
    public string? AdditionalContext { get; set; }
}