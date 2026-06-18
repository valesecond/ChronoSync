using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class SyncResponse : BaseResponse
{
    [JsonPropertyName("t1")]
    public string? T1 { get; set; }

    [JsonPropertyName("t2")]
    public string? T2 { get; set; }
}