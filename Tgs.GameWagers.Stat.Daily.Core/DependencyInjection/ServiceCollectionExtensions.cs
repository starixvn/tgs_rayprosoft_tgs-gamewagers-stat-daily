using System.Reflection;
using Tgs.GameWagers.Stat.Daily.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatDailyCore(
		this IServiceCollection services)
		=> services
		.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
		.AddSingleton<IStatisticalDailyCloseDispatcher, StatisticalDailyCloseDispatcher>()
		.AddSingleton<IStatisticalTimeUtnsil, StatisticalTimeUtnsil>();
}
