using Microsoft.Extensions.Logging;
using Quartz;

namespace Test;

public class TimeJob : IJob
{
	private readonly ILogger _logger;

	public TimeJob(ILogger<TimeJob> logger)
	{
		_logger = logger;
	}
	public Task Execute(IJobExecutionContext context)
	{ 
		_logger.LogInformation(context.ScheduledFireTimeUtc?.ToUniversalTime().ToString());
		return Task.CompletedTask;
	}
}