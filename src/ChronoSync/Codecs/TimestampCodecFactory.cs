namespace ChronoSync.Codecs;

public class TimestampCodecFactory
{
    public ITimestampCodec Create(string encoding)
    {
        return encoding switch
        {
            "Iso8601" => new Iso8601Codec(),

            "Ticks" => new TicksCodec(),

            "TicksBinary" => new TicksBinaryCodec(),

            "TicksBinaryBigEndian" => new TicksBinaryBigEndianCodec(),

            _ => throw new NotSupportedException(
                $"Unsupported encoding: {encoding}")
        };
    }
}