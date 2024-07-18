using Quartz;
using Quartz.Spi;

namespace Test;

public class MyScheduleBuilder : IScheduleBuilder
{
	public IMutableTrigger Build()
	{
		return new MyTrigger
		{
			Name = "mytrigger",
			Group = "mygroup",
			CronExpressionString = "5/10 * * ? * *",
			StartTimeUtc = DateTimeOffset.Now - TimeSpan.FromHours(1),
			MisfireInstruction = MisfireInstruction.SmartPolicy
		};
	}
}