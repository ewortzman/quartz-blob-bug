using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Test
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = Host
				.CreateDefaultBuilder()
				.ConfigureServices((ctx, services) =>
				{
					services.Configure<QuartzOptions>(ctx.Configuration.GetSection("Quartz"));
					services.AddQuartz(q =>
					{
					});
					services.AddQuartzHostedService(opt =>
					{
						opt.WaitForJobsToComplete = true;
					});
				})
				.Build();

			var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
			var scheduler = await schedulerFactory.GetScheduler();

			await scheduler.Clear();
			var job = JobBuilder.Create<MyJob>()
				.WithIdentity("myJob", "mygroup")
				.Build();
			
			var trigger = TriggerBuilder.Create()
				.WithIdentity("myTrigger", "mygroup")
				.StartNow()
				.WithSchedule(new MyScheduleBuilder())
				.Build();

			await scheduler.ScheduleJob(job, trigger);
			
			await builder.RunAsync();
		}
	}
}