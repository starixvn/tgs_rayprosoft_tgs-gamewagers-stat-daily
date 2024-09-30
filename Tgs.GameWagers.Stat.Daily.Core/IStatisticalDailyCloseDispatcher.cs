using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IStatisticalDailyCloseDispatcher
{
	ValueTask PushRequestAsync(StatDailyCloseRequest request, CancellationToken cancellationToken = default);
}
