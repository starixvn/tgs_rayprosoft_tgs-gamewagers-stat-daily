namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IGameWagerStatDailyZoneCloseRepository
{
	ValueTask DeleteBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);

	ValueTask InsertBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}
