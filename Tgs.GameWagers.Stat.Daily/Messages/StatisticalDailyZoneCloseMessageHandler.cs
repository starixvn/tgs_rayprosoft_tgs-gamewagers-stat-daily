using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Hourly.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Messages;

public class StatisticalDailyZoneCloseMessageHandler(
	ILogger<StatisticalDailyZoneCloseMessageHandler> logger,
	IStatisticalDailyZoneCloseDispatcher dispatcher) : IMessageHandler<GameWagersStatHourlyFinishMessage>
{
	public ValueTask HandleAsync(string subject, GameWagersStatHourlyFinishMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return dispatcher.PushCommandAsync(
			command: new StatDailyZoneCloseCommand(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				TimeSpanMinutes: data.TimeSpanMinutes,
				StartOn: data.StartOn.ToDateTimeOffset(),
				EndOn: data.EndOn.ToDateTimeOffset()),
			cancellationToken: cancellationToken);
	}
}
