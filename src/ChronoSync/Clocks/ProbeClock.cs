using ChronoSync.Codecs;

namespace ChronoSync.Clocks;

public class ProbeClock
{
    private readonly ITimestampCodec _codec;

    public ProbeClock(
        string probeName,
        string encoding,
        ITimestampCodec codec,
        double? timeDilationFactor = null)
    {
        ProbeName = probeName;
        Encoding = encoding;
        _codec = codec;
        TimeDilationFactor = timeDilationFactor;
    }

    public string ProbeName { get; }

    public string Encoding { get; }

    public double? TimeDilationFactor { get; }

    public long TimeOffsetTicks { get; private set; }

    public long RoundTripTicks { get; private set; }

    public long SynchronizedLocalTicks { get; private set; }

    public long SynchronizedProbeTicks { get; private set; }

    public void UpdateSynchronization(
        long offsetTicks,
        long roundTripTicks,
        long synchronizedLocalTicks)
    {
        TimeOffsetTicks = offsetTicks;
        RoundTripTicks = roundTripTicks;
        SynchronizedLocalTicks = synchronizedLocalTicks;
        SynchronizedProbeTicks = synchronizedLocalTicks + offsetTicks;
    }

    public long GetCurrentTicks()
    {
        var elapsedTicks = DateTimeOffset.UtcNow.Ticks - SynchronizedLocalTicks;

        if (elapsedTicks < 0)
        {
            elapsedTicks = 0;
        }

        if (TimeDilationFactor.HasValue && TimeDilationFactor.Value > 0)
        {
            elapsedTicks = (long)(elapsedTicks / TimeDilationFactor.Value);
        }

        return SynchronizedProbeTicks + elapsedTicks;
    }

    public string GetCurrentTimestamp()
    {
        return _codec.Encode(GetCurrentTicks());
    }
}