using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Util;
using static Quartz.Logging.OperationName;

namespace Test
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var gmt = TimeZoneUtil.FindTimeZoneById("GMT Standard Time");
			var dstStartDate = new DateTime(2025, 3, 30, 1, 0, 0);
			var dstStart = new DateTimeOffset(dstStartDate, TimeZoneUtil.GetUtcOffset(dstStartDate, gmt));

			var actualProgramStartTime = DateTimeOffset.Now;
			//start application 48 before DST
			var fakeProgramStart = dstStart - TimeSpan.FromHours(48);
			var dstDiff = fakeProgramStart - actualProgramStartTime;

			SystemTime.Now = () =>
			{
				var actualNow = DateTimeOffset.Now;
				var timeSinceStart = actualNow - actualProgramStartTime;
				var mutlipliedTimeSinceStart = (timeSinceStart * 60 * 60);
				var ret = actualProgramStartTime + dstDiff + mutlipliedTimeSinceStart; // 1 hour  per second
				Console.WriteLine(ret.ToUniversalTime().ToString("s"));
				return ret;
			};
			SystemTime.UtcNow = () => TimeZoneUtil.ConvertTime(SystemTime.Now(), TimeZoneInfo.Utc);

			var builder = Host
				.CreateDefaultBuilder()
				.ConfigureServices((ctx, services) =>
				{
					services.Configure<QuartzOptions>(ctx.Configuration.GetSection("Quartz"));
					services.AddQuartz(q => { });
					services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
				})
				.Build();

			var schedulerFactory = builder.Services.GetRequiredService<ISchedulerFactory>();
			var scheduler = await schedulerFactory.GetScheduler();

			await scheduler.Clear();
			await scheduler.ScheduleJob(
				JobBuilder.Create<MyJob>()
					.WithIdentity("myJob", "mygroup")
					.Build(),
				TriggerBuilder.Create()
					.WithIdentity("myTrigger", "mygroup")
					.StartAt(dstStart - TimeSpan.FromDays(1)+TimeSpan.FromMinutes(30)) // start trigger 1 day before DST
					.WithCalendarIntervalSchedule(sched =>
					{
						sched
							.InTimeZone(gmt)
							.PreserveHourOfDayAcrossDaylightSavings(true)
							.SkipDayIfHourDoesNotExist(false)
							.WithIntervalInDays(1);
					})
					.Build());
			//await scheduler.ScheduleJob(
			//	JobBuilder.Create<TimeJob>()
			//		.WithIdentity("timeJob", "mygroup")
			//		.Build(),
			//	TriggerBuilder.Create()
			//		.WithIdentity("timeTrigger", "mygroup")
			//		.StartAt(fakeProgramStart)
			//		.WithSimpleSchedule(sched =>
			//		{
			//			sched
			//				.WithIntervalInHours(2)
			//				.RepeatForever();
			//		})
			//		.Build());

			await builder.RunAsync();
		}
	}
}