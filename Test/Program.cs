using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Util;

namespace Test
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var tz = TimeZoneUtil.FindTimeZoneById("GMT Standard Time");
			var startDateTime = new DateTime(2025, 3, 29, 1, 30, 0);
			var startDto = new DateTimeOffset(startDateTime, TimeZoneUtil.GetUtcOffset(startDateTime, tz));
			var trigger = TriggerBuilder.Create()
				.WithIdentity("myTrigger", "mygroup")
				.StartAt(startDto)
				.WithCalendarIntervalSchedule(sched =>
				{
					sched
						.InTimeZone(tz)
						.PreserveHourOfDayAcrossDaylightSavings(true)
						.SkipDayIfHourDoesNotExist(false)
						.WithIntervalInDays(1);
				})
				.Build();

			DateTimeOffset? next = startDto;
			do
			{
				next = trigger.GetFireTimeAfter(next);
			} while (next < startDto + TimeSpan.FromDays(7));
		}
	}
}