namespace ChronoSync.Helpers;

public static class DateTimeHelper
{
    public static long UtcNowTicks()
    {
        return DateTimeOffset.UtcNow.Ticks;
    }
}