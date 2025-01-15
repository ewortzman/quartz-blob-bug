using Quartz.Impl.Triggers;
using System.Text.Json;

namespace Test;

[Serializable]
public class MyTrigger : CronTriggerImpl
{
	public override bool HasAdditionalProperties => true;
}