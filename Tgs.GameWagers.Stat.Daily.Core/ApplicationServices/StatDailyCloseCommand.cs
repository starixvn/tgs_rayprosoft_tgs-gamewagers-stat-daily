namespace Tgs.GameWagers.Stat.Daily.Core.ApplicationServices;

public record StatDailyCloseCommand(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	int TimeSpanMinutes,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);
