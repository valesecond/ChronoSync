namespace ChronoSync.Codecs;

public class TicksCodec : ITimestampCodec
{
    public string Encode(long ticks)
    {
        return ticks.ToString();
    }

    public long Decode(string value)
    {
        return long.Parse(value);
    }
}