using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppData.Repository
{
	public class SanphamchitietRepository : ISanphamchitietRepository
	{
		private readonly AppDbContext _context;

		public SanphamchitietRepository(AppDbContext context)
		{
			_context = context;
		}
		public void Update(Sanphamchitiet sanphamchitiet)
		{
			// Đánh dấu đối tượng sanphamchitiet là Modified để EF Core biết cần cập nhật
			_context.Sanphamchitiets.Update(sanphamchitiet);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
		public async Task<IEnumerable<Sanphamchitiet>> GetAllAsync()
		{
			return await _context.Sanphamchitiets
								 .Include(sp => sp.Sanpham).Where(sp=>sp.Trangthai == 0|| sp.Trangthai == 1||sp.Trangthai == 2)
								 .ToListAsync();
		}

		public async Task<Sanphamchitiet> GetByIdAsync(int id)
		{
			return await _context.Sanphamchitiets
								 .Include(sp => sp.Sanpham)
								 .FirstOrDefaultAsync(sp => sp.Id == id);
		}

		public async Task AddAsync(Sanphamchitiet entity)
		{
			await _context.Sanphamchitiets.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Sanphamchitiet entity)
		{
			_context.Sanphamchitiets.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _context.Sanphamchitiets.FindAsync(id);
			if (entity != null)
			{
				_context.Sanphamchitiets.Remove(entity);
				await _context.SaveChangesAsync();
			}
		}

		
		public async Task<IEnumerable<Giohangchitiet>> GetGiohangchitietBySpctIdAsync(int spctId)
		{
			return await _context.giohangchitiets
		   .Where(g => g.Idspct == spctId)
		   .ToListAsync();
		}

		public async Task<IEnumerable<Salechitiet>> GetSalechitietBySpctIdAsync(int spctId)
		{
			return await _context.salechitiets
			.Where(s => s.Idspct == spctId)
			.ToListAsync();
		}

		public async Task<IEnumerable<Hoadonchitiet>> GetHoadonchitietBySpctIdAsync(int spctId)
		{
			return await _context.hoadonchitiets
			.Where(h => h.Idspct == spctId)
			.ToListAsync();
		}

		
		public void DeleteGiohangchitiet(Giohangchitiet giohangchitiet)
		{
			_context.giohangchitiets.Remove(giohangchitiet);
		}

		// Phương thức xóa Salechitiet
		public void DeleteSalechitiet(Salechitiet salechitiet)
		{
			_context.salechitiets.Remove(salechitiet);
		}

		// Phương thức xóa Sanphamchitiet
		public void DeleteSanphamchitiet(Sanphamchitiet sanphamchitiet)
		{
			_context.Sanphamchitiets.Remove(sanphamchitiet);
		}
	}
}
