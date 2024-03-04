using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IStatisticalDailyCloseDispatcher
{
	ValueTask PushCommandAsync(StatDailyCloseCommand command, CancellationToken cancellationToken = default);
}
