using ChronoSync.Clients;
using ChronoSync.Clocks;
using ChronoSync.Models;

namespace ChronoSync.Services;

public class JobProcessorService
{
	private readonly ILumaClient _lumaClient;
	private readonly ProbeClockRegistry _clockRegistry;

	public JobProcessorService(
		ILumaClient lumaClient,
		ProbeClockRegistry clockRegistry)
	{
		_lumaClient = lumaClient;
		_clockRegistry = clockRegistry;
	}

	public async Task<bool> ProcessJobsAsync(
		CancellationToken cancellationToken = default)
	{
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			Console.WriteLine("[ChronoSync] Taking next job...");
			var takeJobResponse = await _lumaClient.TakeJobAsync();
			ValidateResponse(takeJobResponse.Code, takeJobResponse.Message);

			if (takeJobResponse.Job == null)
			{
				Console.WriteLine("[ChronoSync] No more jobs available.");
				return true;
			}

			Console.WriteLine($"[ChronoSync] Checking job {takeJobResponse.Job.Id} for probe {takeJobResponse.Job.ProbeName}...");

			var clock = _clockRegistry.GetByName(takeJobResponse.Job.ProbeName);
			var checkRequest = new CheckJobRequest
			{
				ProbeNow = clock.GetCurrentTimestamp(),
				RoundTrip = clock.RoundTripTicks
			};

			var checkResponse = await _lumaClient.CheckJobAsync(
				takeJobResponse.Job.Id,
				checkRequest);

			if (checkResponse.Code.Equals("Done", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine("[ChronoSync] Done received.");
				return true;
			}

			Console.WriteLine($"[ChronoSync] Job result: {checkResponse.Code}");

			ValidateResponse(checkResponse.Code, checkResponse.Message);
		}
	}

	private static void ValidateResponse(
		string code,
		string? message)
	{
		if (code.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase) ||
			code.Equals("Fail", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(
				$"Job processing failed: {code} - {message}");
		}

		if (code.Equals("Error", StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(
				$"Job processing error: {message}");
		}
	}
}
