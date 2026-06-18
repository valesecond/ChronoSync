namespace ChronoSync.Models;

public class BaseResponse
{
    public string Code { get; set; } = string.Empty;

    public string? Message { get; set; }
}