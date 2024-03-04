using Google.Protobuf.WellKnownTypes;

using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Daily.Common;

using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

internal class StatDailyCloseCommandService(
	ILogger<StatDailyCloseCommandService> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatHourlyCloseRepository gameWagerStatHourlyCloseRepository,
	IGameWagerStatDailyCloseRepository gameWagerStatDailyCloseRepository,
	IMessageSender messageSender) : ICommandService<StatDailyCloseCommand>
{
	public async ValueTask ExecuteAsync(StatDailyCloseCommand command, CancellationToken cancellationToken = default)
	{
		var (StartOn, EndOn) = statisticalTimeUtnsil.GetTimeZoneDay(command.StartOn, command.EndOn, 480);

		logger.LogInformation("LogOn:{logOn} Brand:{brandCode} StartOn:{startOn} EndOn:{endOn}",
			DateTime.Now.ToString("o"),
			command.BrandCode,
			StartOn.ToString("o"),
			EndOn.ToString("o"));

		try
		{
			if (await gameWagerStatHourlyCloseRepository.ExistAsync(
				brandId: command.BrandId,
				startOn: StartOn,
				endOn: EndOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				await gameWagerStatDailyCloseRepository.DeleteBatchAsync(
					brandId: command.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await gameWagerStatDailyCloseRepository.InsertBatchAsync(
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
				GameWagersStatDailyNatsConst.StatDailyCloseFinish,
				message);

			await messageSender.PublishAsync(
				subject: GameWagersStatDailyNatsConst.StatDailyCloseFinish,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
