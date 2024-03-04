using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.UnitTest.ApplicationServices;

public class StatDailyZoneCloseCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatDailyZoneCloseCommandService>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyZoneCloseRepository = Substitute.For<IGameWagerStatHourlyZoneCloseRepository>();
		var fakeGameWagerStatDailyZoneCloseRepository = Substitute.For<IGameWagerStatDailyZoneCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatDailyZoneCloseCommand(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480,
			StartOn: new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00)),
			EndOn: new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00)));

		var startOn = new DateTimeOffset(2024, 01, 09, 00, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 10, 00, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneDay(
			startOn: Arg.Is(command.StartOn),
			endOn: Arg.Is(command.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatHourlyZoneCloseRepository.ExistAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var service = new StatDailyZoneCloseCommandService(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyZoneCloseRepository,
			fakeGameWagerStatDailyZoneCloseRepository,
			fakeMessageSender);

		await service.ExecuteAsync(command: command);

		_ = fakeGameWagerStatDailyZoneCloseRepository
			.Received(1)
			.DeleteBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyZoneCloseRepository
			.Received(1)
			.InsertBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		var message = new GameWagersStatDailyFinishMessage
		{
			ResellerId = command.ResellerId,
			ResellerCode = command.ResellerCode,
			LicenseeId = command.LicenseeId,
			LicenseeCode = command.LicenseeCode,
			BrandId = command.BrandId,
			BrandCode = command.BrandCode,
			TimeSpanMinutes = command.TimeSpanMinutes,
			StartOn = startOn.ToTimestamp(),
			EndOn = endOn.ToTimestamp()
		};

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseZoneFinish),
			data: Arg.Is(message),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task No_Data()
	{
		var faceLogger = NullLogger<StatDailyZoneCloseCommandService>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyZoneCloseRepository = Substitute.For<IGameWagerStatHourlyZoneCloseRepository>();
		var fakeGameWagerStatDailyZoneCloseRepository = Substitute.For<IGameWagerStatDailyZoneCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatDailyZoneCloseCommand(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			TimeSpanMinutes: 480,
			StartOn: new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00)),
			EndOn: new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00)));

		var startOn = new DateTimeOffset(2024, 01, 09, 00, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 10, 00, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneDay(
			startOn: Arg.Is(command.StartOn),
			endOn: Arg.Is(command.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatHourlyZoneCloseRepository.ExistAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var service = new StatDailyZoneCloseCommandService(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyZoneCloseRepository,
			fakeGameWagerStatDailyZoneCloseRepository,
			fakeMessageSender);

		await service.ExecuteAsync(command: command);

		_ = fakeGameWagerStatDailyZoneCloseRepository
			.Received(0)
			.DeleteBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyZoneCloseRepository
			.Received(0)
			.InsertBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		var message = new GameWagersStatDailyFinishMessage
		{
			ResellerId = command.ResellerId,
			ResellerCode = command.ResellerCode,
			LicenseeId = command.LicenseeId,
			LicenseeCode = command.LicenseeCode,
			BrandId = command.BrandId,
			BrandCode = command.BrandCode,
			TimeSpanMinutes = command.TimeSpanMinutes,
			StartOn = startOn.ToTimestamp(),
			EndOn = endOn.ToTimestamp()
		};

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseZoneFinish),
			data: Arg.Is(message),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
