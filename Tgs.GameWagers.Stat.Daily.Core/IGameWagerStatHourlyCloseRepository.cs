namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IGameWagerStatHourlyCloseRepository
{
	ValueTask<bool> ExistAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}
