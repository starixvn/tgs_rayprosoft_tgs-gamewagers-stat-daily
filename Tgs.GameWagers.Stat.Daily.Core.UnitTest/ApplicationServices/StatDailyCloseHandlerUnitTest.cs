using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Stat.Daily.Common;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.GameWagers.Stat.Daily.Core.UnitTest.ApplicationServices;

public class StatDailyCloseHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<StatDailyCloseHandler>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var request = new StatDailyCloseRequest(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			StartOn: new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00)),
			EndOn: new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00)));

		var startOn = new DateTimeOffset(2024, 01, 09, 00, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 10, 00, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneDay(
			startOn: Arg.Is(request.StartOn),
			endOn: Arg.Is(request.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatHourlyCloseRepository.ExistAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new StatDailyCloseHandler(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyCloseRepository,
			fakeGameWagerStatDailyCloseRepository,
			fakeMessageSender);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(1)
			.DeleteBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(1)
			.InsertBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseFinish),
			data: Arg.Is(new GameWagersStatDailyFinishMessage
			{
				ResellerId = request.ResellerId,
				ResellerCode = request.ResellerCode,
				LicenseeId = request.LicenseeId,
				LicenseeCode = request.LicenseeCode,
				BrandId = request.BrandId,
				BrandCode = request.BrandCode,
				StartOn = startOn.ToTimestamp(),
				EndOn = endOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task No_Data()
	{
		var faceLogger = NullLogger<StatDailyCloseHandler>.Instance;
		var fakeStatisticalTimeUtnsil = Substitute.For<IStatisticalTimeUtnsil>();
		var fakeGameWagerStatHourlyCloseRepository = Substitute.For<IGameWagerStatHourlyCloseRepository>();
		var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var request = new StatDailyCloseRequest(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			StartOn: new DateTimeOffset(2024, 01, 09, 16, 00, 00, new TimeSpan(08, 00, 00)),
			EndOn: new DateTimeOffset(2024, 01, 09, 17, 00, 00, new TimeSpan(08, 00, 00)));

		var startOn = new DateTimeOffset(2024, 01, 09, 00, 00, 00, new TimeSpan(08, 00, 00));
		var endOn = new DateTimeOffset(2024, 01, 10, 00, 00, 00, new TimeSpan(08, 00, 00));
		_ = fakeStatisticalTimeUtnsil.GetTimeZoneDay(
			startOn: Arg.Is(request.StartOn),
			endOn: Arg.Is(request.EndOn),
			timeSpanMinutes: Arg.Is(480))
			.Returns((startOn, endOn));

		_ = fakeGameWagerStatHourlyCloseRepository.ExistAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var sut = new StatDailyCloseHandler(
			faceLogger,
			fakeStatisticalTimeUtnsil,
			fakeGameWagerStatHourlyCloseRepository,
			fakeGameWagerStatDailyCloseRepository,
			fakeMessageSender);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(0)
			.DeleteBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeGameWagerStatDailyCloseRepository
			.Received(0)
			.InsertBatchAsync(
			brandId: Arg.Is(request.BrandId),
			startOn: Arg.Is(startOn),
			endOn: Arg.Is(endOn),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(GameWagersStatDailyNatsConst.StatDailyCloseFinish),
			data: Arg.Is(new GameWagersStatDailyFinishMessage
			{
				ResellerId = request.ResellerId,
				ResellerCode = request.ResellerCode,
				LicenseeId = request.LicenseeId,
				LicenseeCode = request.LicenseeCode,
				BrandId = request.BrandId,
				BrandCode = request.BrandCode,
				StartOn = startOn.ToTimestamp(),
				EndOn = endOn.ToTimestamp()
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
