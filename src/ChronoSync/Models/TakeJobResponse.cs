using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class TakeJobResponse : BaseResponse
{
    [JsonPropertyName("job")]
    public Job? Job { get; set; }
}