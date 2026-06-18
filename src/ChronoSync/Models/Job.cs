using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class Job
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("probeName")]
    public string ProbeName { get; set; } = string.Empty;
}