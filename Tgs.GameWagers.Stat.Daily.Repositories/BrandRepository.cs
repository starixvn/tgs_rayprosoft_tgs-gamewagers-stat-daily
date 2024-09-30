using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Daily.Core;
using Tgs.GameWagers.Stat.Daily.Core.Models;
using Tgs.GameWagers.Stat.Daily.Entities;

namespace Tgs.GameWagers.Stat.Daily.Repositories;

internal class BrandRepository(
	TgsContext context) : IBrandRepository
{
	public async ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default)
	=> await context.Brands
		.Join(
		context.Licensees,
		brand => brand.LicenseeId,
		licensee => licensee.Id,
		(brand, licensee) => new
		{
			licensee.ResellerId,
			LicenseeId = licensee.Id,
			LicenseeCode = licensee.Code,
			LicenseeStatus = licensee.Status,
			BrandId = brand.Id,
			BrandCode = brand.Code,
			BrandStatus = brand.Status
		})
		.Join(
		context.Resellers,
		conform => conform.ResellerId,
		reseller => reseller.Id,
		(conform, reseller) => new
		{
			conform.ResellerId,
			ResellerCode = reseller.Code,
			ResellerStatus = reseller.Status,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.LicenseeStatus,
			conform.BrandId,
			conform.BrandCode,
			conform.BrandStatus
		})
		.Where(predicate: conform => conform.BrandCode == code)
		.Select(selector: conform => new BrandInfo(
			conform.ResellerId,
			conform.ResellerCode,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.BrandId,
			conform.BrandCode
			))
		.AsAsyncEnumerable()
		.FirstOrDefaultAsync(
		cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
