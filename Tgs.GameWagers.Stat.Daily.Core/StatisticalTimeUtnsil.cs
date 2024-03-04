using Microsoft.Extensions.Options;
using Tgs.GameWagers.Stat.Daily.Core;

namespace Tgs.GameWagers.Stat.Daily.Core;

internal class StatisticalTimeUtnsil : IStatisticalTimeUtnsil
{
	public (DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneDay(DateTimeOffset startOn, DateTimeOffset endOn, int timeSpanMinutes = 480)
	{
		var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
		var startOnTimeSpan = startOn.ToOffset(timeSpan);
		var endOnTimeSpan = endOn.ToOffset(timeSpan).AddDays(+1);
		return new(
			new DateTimeOffset(startOnTimeSpan.Year, startOnTimeSpan.Month, startOnTimeSpan.Day, 00, 00, 00, timeSpan),
			new DateTimeOffset(endOnTimeSpan.Year, endOnTimeSpan.Month, endOnTimeSpan.Day, 00, 00, 00, timeSpan));
	}
}
