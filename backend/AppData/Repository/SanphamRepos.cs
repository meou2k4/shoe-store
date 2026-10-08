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
	public class SanphamRepos : IsanphamRepos
	{
		private readonly AppDbContext _context;

		public SanphamRepos(AppDbContext context)
		{
			_context = context;
		}
		public async Task AddSoluongAsync(int id, int soluongThem)
		{
			var sanpham = await GetByIdAsync(id);
			if (sanpham == null)
			{
				throw new KeyNotFoundException($"Sản phẩm với ID {id} không tồn tại.");
			}
			sanpham.Soluong += soluongThem;
			if (sanpham.Trangthai == 1)
			{
				// Cập nhật Trangthai dựa trên Soluong mới
				sanpham.Trangthai = sanpham.Soluong > 0 ? 0 : 1;
			}

			_context.sanphams.Update(sanpham);
			await _context.SaveChangesAsync();
		}


		public async Task<List<ProductWithAttributesDTO>> GetAllActiveProductsWithAttributesAsync(int id)
		{
			var result = await _context.Sanphamchitiets
				.Where(spct => spct.Trangthai == 0                     // Chi tiết đang hoạt động
							&& spct.Idsp == id)                        // Của sản phẩm cha cần tìm
				.Join(_context.colors, spct => spct.IdMau, c => c.Id,
					  (spct, c) => new { spct, c })
				.Join(_context.chatLieus, sc => sc.spct.IdChatLieu, cl => cl.Id,
					  (sc, cl) => new { sc.spct, sc.c, cl })
				.Join(_context.sizes, scc => scc.spct.IdSize, s => s.Id,
					  (scc, s) => new ProductWithAttributesDTO
					  {
						  Idsp = scc.spct.Idsp,
						  Idspct = scc.spct.Id,
						  Tensp = scc.spct.Sanpham.TenSanpham,   // navigation property
						  Giaban = scc.spct.Sanpham.GiaBan,
						  Giathoidiemhientai = scc.spct.Giathoidiemhientai,
						  Soluong = scc.spct.Soluong,
						  SPCTAttributes = $"{scc.c.Tenmau} - {scc.cl.Tenchatlieu} - {s.Sosize}"
					  })
				.ToListAsync();

			return result;
		}

		public async Task<IEnumerable<Sanpham>> GetAllAsync() => await _context.sanphams.Where(x=>x.Trangthai==0|| x.Trangthai == 1|| x.Trangthai == 2).ToListAsync();

		public async Task<Sanpham> GetByIdAsync(int id) => await _context.sanphams.FindAsync(id);

		public async Task AddAsync(Sanpham sanpham)
		{
			if (sanpham.Soluong > 0)
				sanpham.Trangthai = 0; // Đang bán
			else
				sanpham.Trangthai = 1; // Hết hàng

			_context.sanphams.Add(sanpham);
			await _context.SaveChangesAsync();
		}
		public async Task<IEnumerable<SanphamDetailDto>> GetSanphamDetailsAsync()
		{
			// Step 1: Load data from the database
			var query = await _context.sanphams
				.Where(sp => sp.Trangthai == 0)
				.Join(_context.Sanphamchitiets.Where(spct => spct.Trangthai == 0),
					  sp => sp.Id,
					  spct => spct.Idsp,
					  (sp, spct) => new { sp, spct })
				.Select(x => new
				{
					x.sp.TenSanpham,
					x.spct.Id,
					x.sp.GiaBan,
					x.spct.Soluong,
					x.spct.Color,
					x.spct.ChatLieu,
					x.spct.Size
				})
				.ToListAsync(); // Load all results into memory first

			// Step 2: Group the results in memory
			var groupedResults = query
				.GroupBy(x => new { x.TenSanpham, x.Id, x.GiaBan, x.Soluong, x.Color, x.ChatLieu, x.Size })
				.Select(g => new SanphamDetailDto
				{
					Tensp = g.Key.TenSanpham,
					Idspct = g.Key.Id,
					Giaban = g.Key.GiaBan,
					soluongspct = g.Key.Soluong,
					TenThuocTinhSpct = $"{g.Key.Color.Tenmau} - {g.Key.ChatLieu.Tenchatlieu} - {g.Key.Size.Sosize}"
				});

			return groupedResults;
		}


		public async Task UpdateAsync(Sanpham sanpham)
		{
			_context.sanphams.Update(sanpham);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{

			var sanpham = await GetByIdAsync(id);
			if (sanpham != null)
			{
				_context.sanphams.Remove(sanpham);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Sanpham>> SearchByNameAsync(string name) =>
			await _context.sanphams.Where(sp => sp.TenSanpham.Contains(name)).ToListAsync();
		public async Task<IEnumerable<Sanpham>> SearchByNameHdAsync(string name) =>
			
			await _context.sanphams.Where(sp => sp.TenSanpham.Contains(name) && sp.Trangthai==0).ToListAsync();
		public async Task<int> CountTrangThai0or1Async()
		{
			return await _context.sanphams
								 .AsNoTracking()
								 .CountAsync(sp => sp.Trangthai == 0 || sp.Trangthai == 1);
		}

	}
}