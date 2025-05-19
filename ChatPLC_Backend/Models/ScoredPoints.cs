using System.Text.Json.Serialization;

namespace ChatPLC_Backend.Models;

/// <summary>
/// This is the model that is returned when a user asks a question.
/// </summary>
public class ScoredPoints
{
    // Vector database UUID
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    // The vector point version
    [JsonPropertyName("version")]
    public int Version { get; set; }
    
    // The score of the points vector distance to the query vector 
    [JsonPropertyName("score")]
    public double Score { get; set; }
    
    // Payload - values assigned to the point in key-value pairs
    [JsonPropertyName("payload")]
    public Dictionary<string, string>? Payload { get; set; } = new();
    
    // Vector of the point
    [JsonPropertyName("vector")]
    public List<double>? Vector { get; set; }
    
    // Shard Key
    [JsonPropertyName("shard_key")]
    public string? ShardKey { get; set; }
    
    // Order-by value
    [JsonPropertyName("order_value")]
    public double? OrderValue { get; set; }
}