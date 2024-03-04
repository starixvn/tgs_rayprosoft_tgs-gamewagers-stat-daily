using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Entities;
using Tgs.GameWagers.Stat.Daily.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameWagersStatDailyRepositories(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions)
		=> services
		.AddDbContext<TgsContext>(dbContextOptions)
		.AddScoped<IDbConnection, NpgsqlConnection>((sp) =>
		{
			var context = sp.GetRequiredService<TgsContext>();
			return (NpgsqlConnection)context.Database.GetDbConnection();
		})
		.AddTransient<IBrandRepository, BrandRepository>()
		.AddTransient<IGameWagerStatHourlyCloseRepository, GameWagerStatHourlyCloseRepository>()
		.AddTransient<IGameWagerStatHourlyZoneCloseRepository, GameWagerStatHourlyZoneCloseRepository>()
		.AddTransient<IGameWagerStatDailyCloseRepository, GameWagerStatDailyCloseRepository>()
		.AddTransient<IGameWagerStatDailyZoneCloseRepository, GameWagerStatDailyZoneCloseRepository>();
}
