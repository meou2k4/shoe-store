using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppData.Repository
{
    public class GiamgiaRepos:IGiamgiaRepos
	{
		private readonly AppDbContext _context;
        public GiamgiaRepos(AppDbContext context)
        {
            _context = context;
        }
		public async Task<IEnumerable<Giamgia>> GetAllAsync()
		{
			return await _context.giamgias.Where(x=>x.Trangthai ==0|| x.Trangthai==1|| x.Trangthai ==2|| x.Trangthai==3).ToListAsync();
		}

		public async Task<Giamgia> GetByIdAsync(int id)
		{
			var giamgia = await _context.giamgias.FirstOrDefaultAsync(g => g.Id == id);
			return giamgia;
		}

		public async Task<IEnumerable<Giamgia>> GetVouchersByCustomerIdAsync(int customerId)
		{
			var customer = await _context.khachhangs
				.Include(k => k.Rank)
				.FirstOrDefaultAsync(k => k.Id == customerId);

			if (customer == null)
				return Enumerable.Empty<Giamgia>();

			var rankId = customer.Idrank;

			return await _context.giamgias
				.Include(g => g.Giamgia_Ranks)
				.Where(g => g.Giamgia_Ranks.Any(gr => gr.Idrank == rankId))
				.ToListAsync();
		}
		public async Task<Giamgia> AddAsync(Giamgia giamgia)
		{
			_context.giamgias.Add(giamgia);
			await _context.SaveChangesAsync();
			return giamgia;
		}


		
		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await _context.Database.BeginTransactionAsync();
		}
		public async Task AddRankToGiamgia(int giamgiaId, int rankId)
		{
			var giamgiaRank = new giamgia_rank { IDgiamgia = giamgiaId, Idrank = rankId };
			await _context.giamgia_Ranks.AddAsync(giamgiaRank);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveRankFromGiamgia(int giamgiaId, int rankId)
		{
			var giamgiaRank = await _context.giamgia_Ranks
											.FirstOrDefaultAsync(gr => gr.IDgiamgia == giamgiaId && gr.Idrank == rankId);
			if (giamgiaRank != null)
			{
				_context.giamgia_Ranks.Remove(giamgiaRank);
				await _context.SaveChangesAsync();
			}
		}
		public async Task<List<string>> GetRanksByGiamgiaIdAsync(int giamgiaId)
		{
			return await _context.giamgia_Ranks
				.Where(gr => gr.IDgiamgia == giamgiaId)
				.Join(_context.ranks,
					  gr => gr.Idrank,
					  r => r.Id,
					  (gr, r) => r.Tenrank) // Lấy tên Rank
				.ToListAsync();
		}
		public async Task<bool> DeleteGiamgiaAsync(int giamgiaId)
		{
			// Check if the Giamgia exists in the Hoadon table
			var hoadonExists = await _context.hoadons
				.AnyAsync(h => h.Idgg == giamgiaId);

			if (hoadonExists)
			{
				// Prevent deletion if it exists in the Hoadon table
				return false; // or throw exception based on your requirement
			}

			var giamgiaRankExists = await _context.giamgia_Ranks
				.AnyAsync(gr => gr.IDgiamgia == giamgiaId);

			if (giamgiaRankExists)
			{
				// Delete records from giamgia_rank table that reference this GiamgiaId
				var ranksToDelete = await _context.giamgia_Ranks
					.Where(gr => gr.IDgiamgia == giamgiaId)
					.ToListAsync();

				_context.giamgia_Ranks.RemoveRange(ranksToDelete);
			}
			
			var giamgiaToDelete = await _context.giamgias.FindAsync(giamgiaId);
			if (giamgiaToDelete != null)
			{
				_context.giamgias.Remove(giamgiaToDelete);
				await _context.SaveChangesAsync();
			}

			return true;
		}



		public async Task AddRankToGiamgia(int giamgiaId, List<string> rankNames)
		{
			var giamgia = await _context.giamgias.FindAsync(giamgiaId);
			if (giamgia == null) throw new Exception("Giảm giá không tồn tại");

			foreach (var rankName in rankNames)
			{
				var rank = await _context.ranks.FirstOrDefaultAsync(r => r.Tenrank == rankName);
				if (rank != null)
				{
					var giamgiaRank = new giamgia_rank { IDgiamgia = giamgiaId, Idrank = rank.Id };
					_context.giamgia_Ranks.Add(giamgiaRank);
				}
			}

			await _context.SaveChangesAsync();
		}
		public async Task UpdateRangeAsync(IEnumerable<Giamgia> giamgias)
		{
			_context.giamgias.UpdateRange(giamgias); // Cập nhật nhiều bản ghi
			await _context.SaveChangesAsync() ; 
		}
		public async Task<List<Giamgia>> SearchByDescriptionAsync(string description)
		{
			return await _context.Set<Giamgia>()
								 .Where(g => g.Mota.Contains(description)) // Tìm kiếm mô tả chứa chuỗi
								 .ToListAsync();
		}

		public async Task AddRanksAsync(int giamgiaId, List<int> rankIds)
		{
			var giamgia = await _context.giamgias
				.Include(g => g.Giamgia_Ranks)
				.FirstOrDefaultAsync(g => g.Id == giamgiaId);

			if (giamgia != null)
			{
				// Xóa mối quan hệ cũ
				_context.giamgia_Ranks.RemoveRange(giamgia.Giamgia_Ranks);

				// Thêm mối quan hệ mới
				var newRanks = rankIds.Select(rankId => new giamgia_rank
				{
					IDgiamgia = giamgiaId,
					Idrank = rankId
				}).ToList();

				await _context.giamgia_Ranks.AddRangeAsync(newRanks);
				await _context.SaveChangesAsync();
			}
		}

		public async Task RemoveRanksFromGiamgiaAsync(int giamgiaId)
		{
			var giamgiaRanks = _context.giamgia_Ranks.Where(gr => gr.IDgiamgia == giamgiaId);
			_context.giamgia_Ranks.RemoveRange(giamgiaRanks);
			await _context.SaveChangesAsync();
		}
		public async Task AddRanksToGiamgiaAsync(int giamgiaId, List<int> rankIds)
		{
			var giamgiaRanks = rankIds.Select(rankId => new giamgia_rank
			{
				IDgiamgia = giamgiaId,
				Idrank = rankId
			}).ToList();

			await _context.giamgia_Ranks.AddRangeAsync(giamgiaRanks);
			await _context.SaveChangesAsync();
		}
		public async Task<List<int>> FindRankIdsByNamesAsync(List<string> rankNames)
		{
			return await _context.ranks
				.Where(r => rankNames.Contains(r.Tenrank))
				.Select(r => r.Id)
				.ToListAsync();
		}

		public async Task UpdateAsync(Giamgia giamgia)
		{
			var entry = _context.Entry(giamgia);
			if (entry.State == EntityState.Detached)
			{
				_context.giamgias.Attach(giamgia);
			}
			entry.State = EntityState.Modified;

			await _context.SaveChangesAsync();

		}
		
	}
}
