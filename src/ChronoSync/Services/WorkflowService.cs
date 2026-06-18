using ChronoSync.Clients;
using ChronoSync.Clocks;
using ChronoSync.Models;

namespace ChronoSync.Services;

public class WorkflowService
{
	private readonly ILumaClient _lumaClient;
	private readonly ProbeClockRegistry _clockRegistry;
	private readonly ProbeSyncService _probeSyncService;
	private readonly JobProcessorService _jobProcessorService;

	public WorkflowService(
		ILumaClient lumaClient,
		ProbeClockRegistry clockRegistry,
		ProbeSyncService probeSyncService,
		JobProcessorService jobProcessorService)
	{
		_lumaClient = lumaClient;
		_clockRegistry = clockRegistry;
		_probeSyncService = probeSyncService;
		_jobProcessorService = jobProcessorService;
	}

	public async Task RunAsync(CancellationToken cancellationToken = default)
	{
		while (true)
		{
			var startResponse = await _lumaClient.StartAsync();
			ValidateStartResponse(startResponse);
			Console.WriteLine("[ChronoSync] Test context started.");

			try
			{
				var probeResponse = await _lumaClient.GetProbesAsync();
				ValidateResponse(probeResponse.Code, probeResponse.Message);

				_clockRegistry.Clear();
				Console.WriteLine($"[ChronoSync] {probeResponse.Probes?.Count ?? 0} probe(s) loaded.");

				foreach (var probe in probeResponse.Probes ?? new List<Probe>())
				{
					cancellationToken.ThrowIfCancellationRequested();
					Console.WriteLine($"[ChronoSync] Syncing {probe.Name} ({probe.Encoding})...");

					var clock = await _probeSyncService.SyncAsync(probe, cancellationToken);
					_clockRegistry.Register(clock);
					Console.WriteLine($"[ChronoSync] {probe.Name} synchronized. Offset={clock.TimeOffsetTicks} ticks, RoundTrip={clock.RoundTripTicks} ticks.");
				}

				Console.WriteLine("[ChronoSync] Processing jobs...");
				await _jobProcessorService.ProcessJobsAsync(cancellationToken);
				Console.WriteLine("[ChronoSync] Done response received.");
				return;
			}
			catch (InvalidOperationException ex) when (IsRecoverable(ex))
			{
				Console.WriteLine($"[ChronoSync] Recoverable issue: {ex.Message}");
				Console.WriteLine("[ChronoSync] Restarting context...");
				_clockRegistry.Clear();
			}
		}
	}

	private static void ValidateStartResponse(StartResponse response)
	{
		if (!response.Code.Equals("Success", StringComparison.OrdinalIgnoreCase) ||
			string.IsNullOrWhiteSpace(response.AccessToken))
		{
			throw new InvalidOperationException(
				$"Failed to start workflow: {response.Code} - {response.Message}");
		}
	}

	private static void ValidateResponse(
		string code,
		string? message)
	{
		if (code.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
			code.Equals("Fail", StringComparison.OrdinalIgnoreCase) ||
			code.Equals("Error", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(
				$"Workflow failed: {code} - {message}");
		}
	}

	private static bool IsRecoverable(InvalidOperationException exception)
	{
		return exception.Message.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
			   exception.Message.Contains("Fail", StringComparison.OrdinalIgnoreCase);
	}
}
