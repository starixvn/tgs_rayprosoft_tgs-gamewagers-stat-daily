using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Stat.Daily.Common;

using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

internal class StatDailyCloseHandler(
	ILogger<StatDailyCloseHandler> logger,
	IStatisticalTimeUtnsil statisticalTimeUtnsil,
	IGameWagerStatHourlyCloseRepository gameWagerStatHourlyCloseRepository,
	IGameWagerStatDailyCloseRepository gameWagerStatDailyCloseRepository,
	IMessageSender messageSender) : IRequestHandler<StatDailyCloseRequest>
{
	public async Task Handle(StatDailyCloseRequest request, CancellationToken cancellationToken)
	{
		var timeSpanMinutes = 480;
		var (StartOn, EndOn) = statisticalTimeUtnsil.GetTimeZoneDay(request.StartOn, request.EndOn, timeSpanMinutes);

		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Data = new
				{
					request.ResellerCode,
					request.LicenseeCode,
					request.BrandCode,
					TimeSpanMinutes = timeSpanMinutes,
					StartOn = StartOn.ToString("u"),
					EndOn = EndOn.ToString("u")
				}
			}));

		try
		{
			if (await gameWagerStatHourlyCloseRepository.ExistAsync(
				brandId: request.BrandId,
				startOn: StartOn,
				endOn: EndOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false))
			{
				await gameWagerStatDailyCloseRepository.DeleteBatchAsync(
					brandId: request.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);

				await gameWagerStatDailyCloseRepository.InsertBatchAsync(
					brandId: request.BrandId,
					startOn: StartOn,
					endOn: EndOn,
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Data = new
					{
						request.ResellerCode,
						request.LicenseeCode,
						request.BrandCode,
						TimeSpanMinutes = timeSpanMinutes,
						StartOn = StartOn.ToString("u"),
						EndOn = EndOn.ToString("u")
					},
					ex.Message
				}));
		}
		finally
		{
			var message = new GameWagersStatDailyFinishMessage
			{
				ResellerId = request.ResellerId,
				ResellerCode = request.ResellerCode,
				LicenseeId = request.LicenseeId,
				LicenseeCode = request.LicenseeCode,
				BrandId = request.BrandId,
				BrandCode = request.BrandCode,
				StartOn = StartOn.ToTimestamp(),
				EndOn = EndOn.ToTimestamp()
			};

			logger.LogInformation("{logInformation}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Subject = GameWagersStatDailyNatsConst.StatDailyCloseFinish,
					Data = new
					{
						request.ResellerCode,
						request.LicenseeCode,
						request.BrandCode,
						TimeSpanMinutes = timeSpanMinutes,
						StartOn = StartOn.ToString("u"),
						EndOn = EndOn.ToString("u")
					}
				}));

			await messageSender.PublishAsync(
				subject: GameWagersStatDailyNatsConst.StatDailyCloseFinish,
				data: message,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
