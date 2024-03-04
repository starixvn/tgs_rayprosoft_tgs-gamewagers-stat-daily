using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Entities;

namespace Tgs.GameWagers.Stat.Daily.Repositories;

internal class GameWagerStatDailyZoneCloseRepository(
	ILogger<GameWagerStatDailyZoneCloseRepository> logger,
	IDbConnection connection,
	TgsContext context) : IGameWagerStatDailyZoneCloseRepository
{
	public async ValueTask DeleteBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
	{
		var count = await context.GameWagerStatDailyZoneCloses
			.Where(predicate: predicate => predicate.StatOn >= startOn.UtcDateTime &&
			predicate.StatOn < endOn.UtcDateTime &&
			predicate.BrandId == brandId)
			.ExecuteDeleteAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Message:This brand{brandId} deleted {count} records", DateTime.Now.ToString("o"), brandId, count);

		context.ChangeTracker.Clear();
	}

	public async ValueTask InsertBatchAsync(Guid brandId, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
	{
		DynamicParameters parameters = new DynamicParameters();
		parameters.Add("@startOn", startOn.UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@endOn", endOn.UtcDateTime, DbType.DateTimeOffset, ParameterDirection.Input);
		parameters.Add("@brandId", brandId, DbType.Guid, ParameterDirection.Input);

		var sqlQuery = @"
        insert
	        into
	        dbo.game_wager_stat_daily_zone_closes (reseller_id,
	        licensee_id,
	        brand_id,
	        player_code,
	        game_provider_id,
	        game_id,
	        stat_on,
	        stat_day_on,
	        stat_time_span,
	        count,
	        bet,
	        valid_bet,
	        settle,
	        commission,
	        turnover,
	        win_loss)
        select
	        x.reseller_id,
	        x.licensee_id,
	        x.brand_id,
	        x.player_code,
	        x.game_provider_id,
	        x.game_id,
	        timezone(x.stat_time_span, x.stat_day_on::timestamp) as stat_on,
	        x.stat_day_on,
	        x.stat_time_span,
	        sum(x.count) as count,
	        sum(x.bet) as bet,
	        SUM(x.valid_bet) as valid_bet,
	        SUM(x.settle) as settle,
	        SUM(x.commission) as commission,
	        SUM(x.turnover) as turnover,
	        SUM(x.win_loss) as win_loss
        from
	        dbo.game_wager_stat_hourly_zone_closes x
        where
            stat_on >= @startOn
            and stat_on < @endOn
            and brand_id = @brandId
        group by
	        x.reseller_id,
	        x.licensee_id,
	        x.brand_id,
	        x.player_code,
	        x.game_provider_id,
	        x.game_id,
	        x.stat_day_on,
	        x.stat_time_span;";

		var command = new CommandDefinition(
			commandText: sqlQuery,
			parameters: parameters,
			commandTimeout: (int)TimeSpan.FromMinutes(10).TotalSeconds,
			cancellationToken: cancellationToken);

		var count = await connection.ExecuteAsync(
			command: command)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Message:This brand{brandId} adds {count} records", DateTime.Now.ToString("o"), brandId, count);
	}
}
