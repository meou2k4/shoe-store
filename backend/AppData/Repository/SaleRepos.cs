using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppData.Repository
{
	public class SaleRepos : IsaleRepos
	{
		private readonly AppDbContext _context;
        public SaleRepos(AppDbContext context)
        {
			_context=context;

		}
		public async Task AddSaleAsync(Sale sale)
		{
			await _context.sales.AddAsync(sale);
		}

		public async Task AddAsync(Sale sale)
		{
			await _context.sales.AddAsync(sale);
			await _context.SaveChangesAsync();
		}

		public async Task<List<SaleDetailDTO>> GetSaleDetailsBySaleIdAsync(int saleId)
		{
			var saleDetails = await (from saleDetail in _context.salechitiets
									 join productDetail in _context.Sanphamchitiets on saleDetail.Idspct equals productDetail.Id
									 join product in _context.sanphams on productDetail.Idsp equals product.Id
									 where saleDetail.Idsale == saleId
									 select new SaleDetailDTO
									 {
										 ProductName = product.TenSanpham,
										 Unit = saleDetail.Donvi == 0 ? "VND" : "%",
										 Price = product.GiaBan,
										 Discount = saleDetail.Giatrigiam,
										 Quantity = saleDetail.Soluong,
										 idspct = saleDetail.Idspct,
										 SalePrice = saleDetail.Donvi == 0
											? Math.Max(0, product.GiaBan - saleDetail.Giatrigiam)
											: product.GiaBan * (100 - saleDetail.Giatrigiam) / 100
									 }).ToListAsync();

			return saleDetails;
		}

		public async Task DeleteSaleAsync(Sale sale)
		{
			_context.sales.Remove(sale);
			await Task.CompletedTask;
		}

		public async Task DeleteSaleChitietsBySaleIdAsync(int saleId)
		{
			var saleChitiets = await _context.salechitiets
											.Where(sc => sc.Idsale == saleId)
											.ToListAsync();
			_context.salechitiets.RemoveRange(saleChitiets);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Sale>> GetAllAsync()
		{
			return await _context.sales.ToListAsync();
		}

		public async Task <Sale> GetByIdAsync(int id)
		{
			return await _context.sales.FindAsync(id);
		}
		public async Task<List<UpdateGiaSanphamchitietDTO>> GetSanphamchitietForUpdateAsync(int saleId)
		{
			var query = from sale in _context.sales
						join saleChitiet in _context.salechitiets on sale.Id equals saleChitiet.Idsale
						join spchitiet in _context.Sanphamchitiets on saleChitiet.Idspct equals spchitiet.Id
						select new UpdateGiaSanphamchitietDTO
						{
							Id = spchitiet.Id,
							Giathoidiemhientai = spchitiet.Giathoidiemhientai
						};

			return await query.ToListAsync();
		}
		public async Task UpdateAsync(Sale sale)
		{
			_context.sales.Update(sale);
			await _context.SaveChangesAsync();
		}

		
	}
}
