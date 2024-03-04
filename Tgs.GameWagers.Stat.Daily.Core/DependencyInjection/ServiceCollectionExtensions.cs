using System.Reflection;
using Tgs.GameWagers.Stat.Daily.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatDailyCore(
		this IServiceCollection services)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton<IStatisticalDailyCloseDispatcher, StatisticalDailyCloseDispatcher>()
		.AddSingleton<IStatisticalDailyZoneCloseDispatcher, StatisticalDailyZoneCloseDispatcher>()
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>();
}
