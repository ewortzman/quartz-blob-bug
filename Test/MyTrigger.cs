using Newtonsoft.Json;
using Quartz.Impl.Triggers;

namespace Test;

[Serializable]
public class MyTrigger : CronTriggerImpl
{
	[JsonProperty]
	public DateTimeOffset? NextFireTimeUtc
	{
		get => base.GetNextFireTimeUtc();
		set => base.SetNextFireTimeUtc(value);
	}

	public override DateTimeOffset? GetNextFireTimeUtc()
	{
		return NextFireTimeUtc;
	}

	public override void SetNextFireTimeUtc(DateTimeOffset? fireTime)
	{
		NextFireTimeUtc = fireTime;
	}

	public override bool HasAdditionalProperties => true;
}