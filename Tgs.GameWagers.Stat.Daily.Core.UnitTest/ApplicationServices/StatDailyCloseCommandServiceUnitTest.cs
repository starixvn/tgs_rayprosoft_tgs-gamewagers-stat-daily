using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.UnitTest.ApplicationServices;

public class StatDailyCloseCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatDailyCloseCommandService>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatDailyCloseCommand(
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

		_ = fakeGameWagerStatHourlyCloseRepository.ExistAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var service = new StatDailyCloseCommandService(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyCloseRepository,
			fakeGameWagerStatDailyCloseRepository,
			fakeMessageSender);

		await service.ExecuteAsync(command: command);

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(1)
			.DeleteBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyCloseRepository
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
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseFinish),
			data: Arg.Is(message),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task No_Data()
	{
		var faceLogger = NullLogger<StatDailyCloseCommandService>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new StatDailyCloseCommand(
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

		_ = fakeGameWagerStatHourlyCloseRepository.ExistAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var service = new StatDailyCloseCommandService(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyCloseRepository,
			fakeGameWagerStatDailyCloseRepository,
			fakeMessageSender);

		await service.ExecuteAsync(command: command);

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(0)
			.DeleteBatchAsync(
			brandId: Arg.Is(command.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyCloseRepository
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
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseFinish),
			data: Arg.Is(message),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
