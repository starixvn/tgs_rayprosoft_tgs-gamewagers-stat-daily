using MediatR;

namespace Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

public record StatDailyCloseRequest(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn) : IRequest;
