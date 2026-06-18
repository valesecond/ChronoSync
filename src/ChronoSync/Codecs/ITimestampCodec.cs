namespace ChronoSync.Codecs;

public interface ITimestampCodec
{
    string Encode(long ticks);

    long Decode(string value);
}