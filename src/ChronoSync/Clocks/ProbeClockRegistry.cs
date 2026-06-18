namespace ChronoSync.Clocks;

public class ProbeClockRegistry
{
    private readonly Dictionary<string, ProbeClock> _clocks =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register(ProbeClock clock)
    {
        _clocks[clock.ProbeName] = clock;
    }

    public ProbeClock GetByName(string probeName)
    {
        if (!_clocks.TryGetValue(probeName, out var clock))
        {
            throw new KeyNotFoundException(
                $"Probe clock not found: {probeName}");
        }

        return clock;
    }

    public IReadOnlyCollection<ProbeClock> GetAll()
    {
        return _clocks.Values.ToList();
    }

    public bool Contains(string probeName)
    {
        return _clocks.ContainsKey(probeName);
    }

    public void Clear()
    {
        _clocks.Clear();
    }
}