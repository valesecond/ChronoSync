namespace ChronoSync.Configuration;

public class AppSettings
{
    public LumaSettings Luma { get; set; } = new();
}

public class LumaSettings
{
    public string BaseUrl { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Level { get; set; } = 1;
}