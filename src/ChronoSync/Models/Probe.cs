using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class Probe
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("encoding")]
    public string Encoding { get; set; } = string.Empty;

    [JsonPropertyName("timeDilationFactor")]
    public double? TimeDilationFactor { get; set; }
}