using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IRepository
{
    public interface IHoadonRepository
    {
		Task<IEnumerable<Hoadon>> GetAlOlnlAsync();
		List<Hoadon> GetAllHoadons();
		Task<Hoadon> GetHoaDonById(int id);
		Task AddHoaDon(Hoadon hoadon);
		Task UpdateHoaDon(Hoadon hoadon);
		Task<Hoadonchitiet> AddHoaDonChiTiet(Hoadonchitiet hoadonChiTiet);
		Task AddHoaDonWithDetails(Hoadon hoadon, List<Hoadonchitiet> hoadonChiTiets);
		Task<List<Hoadon>> GetAllAsync();
		Task SaveChanges();
		Task<Hoadon> GetByIdAsync(int id);
		Task UpdateAsync(Hoadon hoadon);
		Task SaveChangesAsync();
		Task<IEnumerable<Hoadon>> GetAllOffAsync();
		Task<(decimal TongTienThanhToan, int TongSoLuongDonHang)> GetDailyReportAsync(DateTime date);
		IEnumerable<HoadonSummaryDto> GetOrderSummaryByTime(string timeUnit);
		Task DeleteHoadonAsync(Hoadon hoadon);
		Task DeleteHoadonchitietsAsync(IEnumerable<Hoadonchitiet> hoadonchitiets);
	

		Task<List<HoadonReportDto>> GetOlnOrdersByWeekAsync();
		Task<List<HoadonReportDto>> GetOffOrdersByWeekAsync();
		Task<int> CountDonHangAsync();
		Task<decimal> SumDoanhThuThanhCongAsync();
		Task<List<LatestInvoiceDto>> Get10LatestInvoicesAsync();
	}
}
