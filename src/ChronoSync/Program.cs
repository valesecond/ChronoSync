using ChronoSync.Extensions;
using ChronoSync.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

using var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
	eventArgs.Cancel = true;
	cancellationSource.Cancel();
	Console.WriteLine();
	Console.WriteLine("[ChronoSync] Cancellation requested. Finishing current operation...");
	Console.ResetColor();
};

WriteBanner();

try
{
	Console.WriteLine("[ChronoSync] Starting workflow...");

	var builder = Host.CreateApplicationBuilder(args);
	builder.Configuration.AddJsonFile(
		Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
		optional: false,
		reloadOnChange: false);
	builder.Services.AddChronoSync(builder.Configuration);

	using var host = builder.Build();

	var workflowService = host.Services.GetRequiredService<WorkflowService>();
	await workflowService.RunAsync(cancellationSource.Token);

	Console.ForegroundColor = ConsoleColor.Green;
	Console.WriteLine("[ChronoSync] Workflow completed successfully.");
	Console.ResetColor();
}
catch (OperationCanceledException)
{
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine("[ChronoSync] Execution canceled by user.");
	Console.ResetColor();
	Environment.ExitCode = 1;
}
catch (HttpRequestException exception)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("[ChronoSync] Network error while calling Luma APIs.");
	Console.ResetColor();
	Console.Error.WriteLine(exception.Message);
	Environment.ExitCode = 1;
}
catch (Exception exception)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine("[ChronoSync] Workflow failed.");
	Console.ResetColor();
	Console.Error.WriteLine(exception.Message);

	if (exception.InnerException != null)
	{
		Console.Error.WriteLine(exception.InnerException.Message);
	}

	Environment.ExitCode = 1;
}

static void WriteBanner()
{
	Console.ForegroundColor = ConsoleColor.Cyan;
	Console.WriteLine("============================================");
	Console.WriteLine("              ChronoSync");
	Console.WriteLine("============================================");
	Console.ResetColor();
	Console.WriteLine("Luma clock synchronization runner");
	Console.WriteLine();
	Console.WriteLine("Tip: press Ctrl+C to cancel safely.");
	Console.WriteLine();
}
