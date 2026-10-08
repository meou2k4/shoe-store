using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppData.Repository
{
	public class RankRepository: IRankRepository
	{
		private readonly AppDbContext _context;

		public RankRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddRankAsync(Rank rank)
		{
			await _context.ranks.AddAsync(rank);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Rank>> GetAllRanksAsync()
		{
			return await _context.ranks.Where(x=>x.Trangthai ==0||x.Trangthai==1).ToListAsync();
		}
		public async Task<IEnumerable<Rank>> FindByNameAsync(string name)
		{
			return await _context.ranks
				.Where(r => r.Tenrank.Contains(name)&&( r.Trangthai == 0 || r.Trangthai == 1))
				.ToListAsync();
		}

		public async Task<Rank> GetRankByIdAsync(int id)
		{
			return await _context.ranks.FindAsync(id);
		}

		public async Task UpdateRankAsync(Rank rank)
		{
			_context.ranks.Update(rank);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteRankAsync(int id)
		{
			var rank = await GetRankByIdAsync(id);
			if (rank != null)
			{
				_context.ranks.Remove(rank);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<IEnumerable<Rank>> SearchRanksAsync(string keyword)
		{
			return await _context.ranks
				.Where(r => r.Tenrank.Contains(keyword))
				.ToListAsync();
		}
		public async Task<Rank> GetRankByNameAsync(string rankName)
		{
			return await _context.ranks
				.FirstOrDefaultAsync(r => r.Tenrank == rankName);
		}


		public async Task<bool> CheckForeignKeyConstraintAsync(int Id)
		{
			var x = await _context.giamgia_Ranks
				.AnyAsync(h => h.Idrank == Id);  
			var y = await _context.khachhangs
				.AnyAsync(h => h.Idrank == Id);
			return x || y;
		}
	}
}
