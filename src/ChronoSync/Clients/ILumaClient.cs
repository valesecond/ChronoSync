using ChronoSync.Models;

namespace ChronoSync.Clients;

public interface ILumaClient
{
    Task<StartResponse> StartAsync();

    Task<ProbeResponse> GetProbesAsync();

    Task<SyncResponse> SyncProbeAsync(string probeId);

    Task<TakeJobResponse> TakeJobAsync();

    Task<CheckJobResponse> CheckJobAsync(
        string jobId,
        CheckJobRequest request);
}