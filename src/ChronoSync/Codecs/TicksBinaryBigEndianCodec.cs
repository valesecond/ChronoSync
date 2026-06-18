namespace ChronoSync.Codecs;

public class TicksBinaryBigEndianCodec : ITimestampCodec
{
    public string Encode(long ticks)
    {
        var bytes = BitConverter.GetBytes(ticks);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return Convert.ToBase64String(bytes);
    }

    public long Decode(string value)
    {
        var bytes = Convert.FromBase64String(value);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return BitConverter.ToInt64(bytes, 0);
    }
}