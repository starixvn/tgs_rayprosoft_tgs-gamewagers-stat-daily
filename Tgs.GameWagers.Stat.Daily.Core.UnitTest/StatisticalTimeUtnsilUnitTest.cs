using Microsoft.Extensions.Options;

namespace Tgs.GameWagers.Stat.Daily.Core.UnitTest;

public class StatisticalTimeUtnsilUnitTest
{
	[Fact]
	public void Get_TimeZone_Day_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Minus_240()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneDay(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), -240);

		Assert.Equal(DateTimeOffset.Parse("2023-11-20T00:00:00-04:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-21T00:00:00-04:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Day_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_330()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneDay(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 330);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T00:00:00+05:30").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-22T00:00:00+05:30").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Day_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_540()
	{
		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneDay(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 540);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T00:00:00+09:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-22T00:00:00+09:00").UtcDateTime, EndOn.UtcDateTime);
	}

	[Fact]
	public void Get_TimeZone_Day_From_23_11_21_10_56_08_To_23_11_21_11_04_08_TimeSpanMinutes_Plus_480()
	{

		var sut = new StatisticalTimeUtnsil();

		var (StartOn, EndOn) = sut.GetTimeZoneDay(DateTimeOffset.Parse("2023-11-21T10:56:00+08:00"), DateTimeOffset.Parse("2023-11-21T11:04:00+08:00"), 480);

		Assert.Equal(DateTimeOffset.Parse("2023-11-21T00:00:00+08:00").UtcDateTime, StartOn.UtcDateTime);
		Assert.Equal(DateTimeOffset.Parse("2023-11-22T00:00:00+08:00").UtcDateTime, EndOn.UtcDateTime);
	}
}