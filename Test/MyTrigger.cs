using Quartz.Impl.Triggers;

namespace Test;

[Serializable]
public class MyTrigger : CronTriggerImpl
{
	public override bool HasAdditionalProperties => true;
}