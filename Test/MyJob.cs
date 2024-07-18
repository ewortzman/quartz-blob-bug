using Microsoft.Extensions.Logging;
using Quartz;

namespace Test;

public class MyJob : IJob
{
	private readonly ILogger _logger;

	public MyJob(ILogger<MyJob> logger)
	{
		_logger = logger;
	}

	public Task Execute(IJobExecutionContext context)
	{
		_logger.LogInformation("Running...");
		return Task.CompletedTask;
	}
}