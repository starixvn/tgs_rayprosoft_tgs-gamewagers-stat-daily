namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IGameWagerStatHourlyZoneCloseRepository
{
	ValueTask<bool> ExistAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}
