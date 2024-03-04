using Tgs.GameWagers.Stat.Daily.Core.Models;

namespace Tgs.GameWagers.Stat.Daily.Core;

public interface IBrandRepository
{
	ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default);
}
