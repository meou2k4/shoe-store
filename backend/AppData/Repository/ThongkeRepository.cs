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
	public class ThongkeRepository : IThongkesanphamRepository
	{
		private readonly	AppDbContext _context;
        public ThongkeRepository(AppDbContext context)
        {
            _context = context;
        }

		public async Task<List<CustomerStatisticsDTO>> GetTop5CustomersAsync(DateTime month)
		{
			var topCustomers = await _context.hoadons
				.Where(h => h.Trangthai == 3 && h.Thoigiandathang.Month == month.Month && h.Thoigiandathang.Year == month.Year) // Lọc theo đơn hàng thành công trong tháng
				.GroupBy(h => h.Khachhang)
				.Select(g => new CustomerStatisticsDTO
				{
					CustomerName = g.Key.Ten,
					TotalOrders = g.Count(),
					TotalSpent = g.Sum(x => x.Tongtiencantra),
					SuccessfulOrders = g.Count(x => x.Trangthai == 3),
					CanceledOrders = g.Count(x => x.Trangthai == 4),
					ReturnedOrders = g.Count(x => x.Trangthai == 5)
				})
				.OrderByDescending(c => c.TotalSpent)
				.Take(5)
				.ToListAsync();

			return topCustomers;
		}

		public async Task<List<TopSellingProductDto>> GetTopSellingProductsByTimeAsync(DateTime startDate, DateTime endDate)
		{
			var topSellingProducts = await (from hdct in _context.hoadonchitiets
											join spct in _context.Sanphamchitiets on hdct.Idspct equals spct.Id
											join sp in _context.sanphams on spct.Idsp equals sp.Id
											join hd in _context.hoadons on hdct.Idhd equals hd.Id
											where hd.Trangthaidonhang == 3  
											&&( ( hd.Trangthai == 0 && hd.Ngaygiaothucte >= startDate && hd.Ngaygiaothucte <= endDate )|| (hd.Trangthai == 1 && hd.Thoigiandathang >= startDate && hd.Thoigiandathang <= endDate) ) // Lọc theo khoảng thời gian
											group hdct by new { sp.Id, sp.TenSanpham } into grouped
											select new TopSellingProductDto
											{
												Idsp = grouped.Key.Id,
												Tensp = grouped.Key.TenSanpham,
												TotalSales = grouped.Sum(x => x.Soluong)
											})
											.OrderByDescending(x => x.TotalSales)
											.Take(5).ToListAsync();  

			return topSellingProducts;
		}
		public async Task<List<Khachhang>> GetActiveCustomersAsync()
		{
			var activeCustomers = await _context.khachhangs
				.Where(kh => kh.Trangthai == 0) 
				.ToListAsync();

			return activeCustomers;
		}

	}
}
