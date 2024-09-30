using System.Text.Json;
using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Messages;

public class StatisticalDailyCloseMessageHandler(
	ILogger<StatisticalDailyCloseMessageHandler> logger,
	IStatisticalDailyCloseDispatcher dispatcher) : IMessageHandler<GameWagersStatHourlyFinishMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatHourlyFinishMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.ResellerCode,
					data.LicenseeCode,
					data.BrandCode,
					StartOn = data.StartOn.ToDateTimeOffset().ToString("u"),
					EndOn = data.EndOn.ToDateTimeOffset().ToString("u")
				},
				Message = "This receive message handler."
			}));

		return dispatcher.PushRequestAsync(
			request: new StatDailyCloseRequest(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
