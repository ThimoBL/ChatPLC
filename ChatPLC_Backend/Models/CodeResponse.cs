using System.Text.Json.Serialization;

namespace ChatPLC_Backend.Models;

public class CodeResponse
{
    /// <summary>
    /// The code snippet returned by the model and accepted by the user.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = null!;
}