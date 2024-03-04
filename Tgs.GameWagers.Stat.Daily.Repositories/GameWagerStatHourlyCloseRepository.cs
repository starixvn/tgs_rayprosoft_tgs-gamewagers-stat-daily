using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Entities;

namespace Tgs.GameWagers.Stat.Daily.Repositories;

internal class GameWagerStatHourlyCloseRepository(
	TgsContext context) : IGameWagerStatHourlyCloseRepository
{
	public async ValueTask<bool> ExistAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
		=> await context.GameWagerStatHourlyCloses.AnyAsync(
			predicate:
			predicate => predicate.StatOn >= startOn.UtcDateTime &&
			predicate.StatOn < endOn.UtcDateTime &&
			predicate.BrandId == brandId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
