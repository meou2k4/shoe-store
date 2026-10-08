using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IService_Admin
{
	public interface IHoadonService
	{
		
		Task<IEnumerable<Hoadon>> GetAllHoadonsAsync();
		List<Hoadon> GetAllHoadons();
		Task<Hoadon> AddHoaDon(CreateHoadonDTO dto);
		Task<Hoadon> AddHoaDonKhachhangthanthietoff(HoadonoffKhachhangthanthietDto dto, int diemSuDung);
		Task<List<Hoadon>> GetAllAsync();
		Task<IEnumerable<Hoadon>> GetAllHoadonsOlnAsync();
		Task<HoadonupdatetrangthaiDto> ChuyenTrangThaiAsync(int id, int huy);
		Task<HoadonupdatetrangthaiDto> RestoreStateAsync(int id, int trangthai);
		Task<Hoadon> GetByIdAsync(int id);
		Task<IEnumerable<Hoadon>> GetAllOffHoadonsAsync();
		Task<(decimal TongTienThanhToan, int TongSoLuongDonHang)> GetDailyReportAsync(DateTime date);
		IEnumerable<HoadonSummaryDto> GetOrderSummary(string timeUnit);
		Task<List<HoadonReportDto>> GetOlnOrdersByWeekAsync();
		Task<List<HoadonReportDto>> GetOffOrdersByWeekAsync();
		Task<int> GetSoDonAsync();
		Task<decimal> GetDoanhThuThanhCongAsync();
		Task<List<LatestInvoiceDto>> Get10LatestInvoicesAsync();
	}
}
