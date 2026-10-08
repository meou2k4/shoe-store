using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Models;

namespace AppData.IRepository
{
	public interface IRankRepository
	{
		Task AddRankAsync(Rank rank);
		Task<IEnumerable<Rank>> GetAllRanksAsync();
		Task<Rank> GetRankByIdAsync(int id);
		Task UpdateRankAsync(Rank rank);
		Task DeleteRankAsync(int id);
		Task<Rank> GetRankByNameAsync(string rankName);
		Task<IEnumerable<Rank>> FindByNameAsync(string name);
		Task<IEnumerable<Rank>> SearchRanksAsync(string keyword);
		Task<bool> CheckForeignKeyConstraintAsync(int khachhangId);
	}
}
