using System.Text.Json.Serialization;

namespace ChronoSync.Models;

public class StartResponse : BaseResponse
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }
}