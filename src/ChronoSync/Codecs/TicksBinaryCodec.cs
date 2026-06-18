namespace ChronoSync.Codecs;

public class TicksBinaryCodec : ITimestampCodec
{
    public string Encode(long ticks)
    {
        var bytes = BitConverter.GetBytes(ticks);

        return Convert.ToBase64String(bytes);
    }

    public long Decode(string value)
    {
        var bytes = Convert.FromBase64String(value);

        return BitConverter.ToInt64(bytes, 0);
    }
}