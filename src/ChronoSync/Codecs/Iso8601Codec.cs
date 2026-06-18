using System.Globalization;

namespace ChronoSync.Codecs;

public class Iso8601Codec : ITimestampCodec
{
    public string Encode(long ticks)
    {
        var date = new DateTimeOffset(ticks, TimeSpan.Zero);

        return date.ToString(
            "yyyy-MM-ddTHH:mm:ss.fffffffzzz",
            CultureInfo.InvariantCulture);
    }

    public long Decode(string value)
    {
        return DateTimeOffset
            .Parse(value, CultureInfo.InvariantCulture)
            .Ticks;
    }
}