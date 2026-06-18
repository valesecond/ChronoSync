using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class ProbeResponse : BaseResponse
{
    [JsonPropertyName("probes")]
    public List<Probe>? Probes { get; set; }
}