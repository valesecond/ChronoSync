using ChronoSync.Clients;
using ChronoSync.Clocks;
using ChronoSync.Codecs;
using ChronoSync.Configuration;
using ChronoSync.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChronoSync.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddChronoSync(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddOptions<AppSettings>()
			.Configure(options =>
			{
				configuration.GetSection("Luma").Bind(options.Luma);
			});

		services.AddHttpClient();
		services.AddSingleton<ILumaClient>(serviceProvider =>
			new LumaClient(
				serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(),
				serviceProvider.GetRequiredService<IOptions<AppSettings>>()));
		services.AddSingleton<TimestampCodecFactory>();
		services.AddSingleton<ProbeClockRegistry>();
		services.AddSingleton<ProbeSyncService>();
		services.AddSingleton<JobProcessorService>();
		services.AddSingleton<WorkflowService>();

		return services;
	}
}
