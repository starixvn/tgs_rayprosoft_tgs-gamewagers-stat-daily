namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IStatisticalTimeUtnsil
{
	(DateTimeOffset StartOn, DateTimeOffset EndOn) GetTimeZoneDay(DateTimeOffset startOn, DateTimeOffset endOn, int timeSpanMinutes = 480);
}
