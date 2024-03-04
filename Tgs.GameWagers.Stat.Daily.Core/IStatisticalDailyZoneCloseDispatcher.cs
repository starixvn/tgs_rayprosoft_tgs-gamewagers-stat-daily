using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IStatisticalDailyZoneCloseDispatcher
{
	ValueTask PushCommandAsync(StatDailyZoneCloseCommand command, CancellationToken cancellationToken = default);
}
