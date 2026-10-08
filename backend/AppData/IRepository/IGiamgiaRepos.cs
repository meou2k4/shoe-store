using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppData.IRepository
{
	public interface IGiamgiaRepos
	{

		Task<IEnumerable<Giamgia>> GetVouchersByCustomerIdAsync(int customerId);
		Task<IEnumerable<Giamgia>> GetAllAsync();
		Task<Giamgia> GetByIdAsync(int id);
		Task<Giamgia> AddAsync(Giamgia giamgia);
		Task UpdateAsync(Giamgia giamgia);
		Task<List<Giamgia>> SearchByDescriptionAsync(string description);
		Task AddRankToGiamgia(int giamgiaId, List<string> rankNames);
		Task<bool> DeleteGiamgiaAsync(int giamgiaId);
		Task AddRankToGiamgia(int giamgiaId, int rankId);
		Task RemoveRankFromGiamgia(int giamgiaId, int rankId);
		Task<List<string>> GetRanksByGiamgiaIdAsync(int giamgiaId);
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task UpdateRangeAsync(IEnumerable<Giamgia> giamgias);
		Task<List<int>> FindRankIdsByNamesAsync(List<string> rankNames);
		Task AddRanksToGiamgiaAsync(int giamgiaId, List<int> rankIds);


	}
}
