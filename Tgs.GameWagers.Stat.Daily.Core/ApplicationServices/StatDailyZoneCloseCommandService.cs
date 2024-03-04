using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

internal class StatDailyZoneCloseCommandService(
	ILogger<StatDailyZoneCloseCommandService> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatHourlyZoneCloseRepository gameWagerStatHourlyZoneCloseRepository,
	IGameWagerStatDailyZoneCloseRepository gameWagerStatDailyZoneCloseRepository,
	IMessageSender messageSender) : ICommandService<StatDailyZoneCloseCommand>
{
	public async ValueTask ExecuteAsync(StatDailyZoneCloseCommand command, CancellationToken cancellationToken = default)
	{
		var (StartOn, EndOn) = statisticalTimeUtnsil.GetTimeZoneDay(command.StartOn, command.EndOn, command.TimeSpanMinutes);

		logger.LogInformation("LogOn:{logOn} Brand:{brandCode} StartOn:{startOn} EndOn:{endOn}",
			DateTime.Now.ToString("o"),
			command.BrandCode,
			StartOn.ToString("o"),
			EndOn.ToString("o"));

		try
		{
			if (await gameWagerStatHourlyZoneCloseRepository.ExistAsync(
				brandId: command.BrandId,
				startOn: StartOn,
				endOn: EndOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				await gameWagerStatDailyZoneCloseRepository.DeleteBatchAsync(
					brandId: command.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await gameWagerStatDailyZoneCloseRepository.InsertBatchAsync(
					brandId: command.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Message: This exception occurs.({command})", DateTime.Now.ToString("o"), command);
		}
		finally
		{
			var message = new GameWagersStatDailyFinishMessage
			{
				ResellerId = command.ResellerId,
				ResellerCode = command.ResellerCode,
				LicenseeId = command.LicenseeId,
				LicenseeCode = command.LicenseeCode,
				BrandId = command.BrandId,
				BrandCode = command.BrandCode,
				TimeSpanMinutes = command.TimeSpanMinutes,
				StartOn = StartOn.ToTimestamp(),
				EndOn = EndOn.ToTimestamp()
			};

			logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:The content of this message.({message})",
				DateTime.Now.ToString("u"),
				GameWagersStatDailyNatsConst.StatDailyCloseZoneFinish,
				message);

			await messageSender.PublishAsync(
				subject: GameWagersStatDailyNatsConst.StatDailyCloseZoneFinish,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
