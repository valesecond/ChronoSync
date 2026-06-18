using ChronoSync.Clients;
using ChronoSync.Clocks;
using ChronoSync.Codecs;
using ChronoSync.Helpers;
using ChronoSync.Models;

namespace ChronoSync.Services;

public class ProbeSyncService
{
	private readonly ILumaClient _lumaClient;
	private readonly TimestampCodecFactory _codecFactory;

	public ProbeSyncService(
		ILumaClient lumaClient,
		TimestampCodecFactory codecFactory)
	{
		_lumaClient = lumaClient;
		_codecFactory = codecFactory;
	}

	public async Task<ProbeClock> SyncAsync(
		Probe probe,
		CancellationToken cancellationToken = default)
	{
		var codec = _codecFactory.Create(probe.Encoding);
		var clock = new ProbeClock(
			probe.Name,
			probe.Encoding,
			codec,
			probe.TimeDilationFactor);

		var accumulatedOffsetTicks = 0L;

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var localTicks = DateTimeOffset.UtcNow.Ticks + clock.TimeOffsetTicks;
			var t0 = localTicks;
			var response = await _lumaClient.SyncProbeAsync(probe.Id);
			var t3 = DateTimeOffset.UtcNow.Ticks + clock.TimeOffsetTicks;

			ValidateSyncResponse(response);

			if (response.Code.Equals("ProbeUnreachable", StringComparison.OrdinalIgnoreCase))
			{
				await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
				continue;
			}

			var t1 = codec.Decode(response.T1!);
			var t2 = codec.Decode(response.T2!);

			var newOffsetTicks = ((t1 - t0) + (t2 - t3)) / 2;
			var roundTripTicks = (t3 - t0) - (t2 - t1);

			accumulatedOffsetTicks += newOffsetTicks;

			clock.UpdateSynchronization(
				accumulatedOffsetTicks,
				roundTripTicks,
				t3);

			if (Math.Abs(newOffsetTicks) < TickConstants.SyncThreshold)
			{
				return clock;
			}
		}
	}

	private static void ValidateSyncResponse(SyncResponse response)
	{
		if (response.Code.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
			response.Code.Equals("Fail", StringComparison.OrdinalIgnoreCase) ||
			response.Code.Equals("Error", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(
				$"Sync failed: {response.Code} - {response.Message}");
		}

		if (!response.Code.Equals("ProbeUnreachable", StringComparison.OrdinalIgnoreCase) &&
			(string.IsNullOrWhiteSpace(response.T1) || string.IsNullOrWhiteSpace(response.T2)))
		{
			throw new InvalidOperationException(
				"Sync response did not contain t1/t2 values.");
		}
	}
}
