using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.IService;
using AppData.IService_Admin;
using AppData.Models;
using AppData.Repository;
using Microsoft.EntityFrameworkCore;

namespace AppData.Service_Admin
{
	public class HoadonService : IHoadonService
	{
		private readonly IHoadonRepository _hoadonRepository;
		private readonly IGiamgiaRepos _giamgiaRepository;
		private readonly Ikhachhangrepository _khachhangRepository;
		private readonly IsanphamRepos _sanphamRepository;
		private readonly ISanphamchitietRepository _sanphamctRepository;
		private readonly IRankRepository _rankRepository;

		public HoadonService(IHoadonRepository hoadonRepository, IGiamgiaRepos giamgiaRepository, Ikhachhangrepository khachhangRepository, IsanphamRepos sanphamRepository, ISanphamchitietRepository sanphamctRepository, IRankRepository rankRepository)
		{
			_hoadonRepository = hoadonRepository;
			_giamgiaRepository = giamgiaRepository;
			_khachhangRepository = khachhangRepository;
			_sanphamctRepository = sanphamctRepository;
			_sanphamRepository = sanphamRepository;
			_rankRepository = rankRepository;
		}
		public async Task<List<HoadonReportDto>> GetOlnOrdersByWeekAsync()
		{
			return await _hoadonRepository.GetOlnOrdersByWeekAsync();
		}

		public async Task<List<HoadonReportDto>> GetOffOrdersByWeekAsync()
		{
			return await _hoadonRepository.GetOffOrdersByWeekAsync();
		}
		public async Task<HoadonupdatetrangthaiDto> ChuyenTrangThaiAsync(int id, int huy)
		{
			var hoadon = await _hoadonRepository.GetByIdAsync(id);

			if (hoadon == null)
			{
				throw new Exception("Hóa đơn không tồn tại.");
			}

			// Logic chuyển trạng thái
			switch (hoadon.Trangthaidonhang)
			{
				case 0:
					hoadon.Trangthaidonhang = (huy == 0) ? 1 : 4;
					break;
				case 1:
					hoadon.Trangthaidonhang = (huy == 0) ? 2 : 4;
					break;
				case 2:
					hoadon.Trangthaidonhang = (huy == 0) ? 3 : 4;
				
					break;
				default:
					throw new InvalidOperationException("Trạng thái không hợp lệ.");
			}

			// Cập nhật hóa đơn
			await _hoadonRepository.UpdateAsync(hoadon);

			// Trả về DTO
			return new HoadonupdatetrangthaiDto	
			{
				Id = hoadon.Id,
				Trangthai = hoadon.Trangthai
			};
		}
		public async Task<Hoadon> AddHoaDon(CreateHoadonDTO dto)
		{
			// Tạo hóa đơn
			var hoadon = new Hoadon
			{
				Idnv = dto.Idnv,
				Idpttt = dto.Idpttt,
				Idkh = null,
				Idgg = null,
				Diachiship = "",
				Ngaygiaothucte = null,
				Sdt = "",
				Ghichu = dto.Ghichu,
				Thoigiandathang = DateTime.Now,
				Tongtiencantra = dto.Tongtiencantra,
				Tongtiensanpham = dto.Tongtiensanpham,
				Tonggiamgia = 0,
				Trangthaidonhang = 3,
				Trangthaithanhtoan = 0,
				Trangthai = 1,
				Phivanchuyen = 0,
			};

			var hoadonChiTiets = new List<Hoadonchitiet>();
			foreach (var sp in dto.SanPhamChiTiet)
			{
				var hoadonChiTiet = new Hoadonchitiet
				{
					Idhd = hoadon.Id,
					Idspct = sp.Idspct,
					Soluong = sp.Soluong,
					Giasp = sp.Giasp,
					Giamgia = sp.Giamgia ?? 0
				};

				hoadonChiTiets.Add(hoadonChiTiet);
				var x = await _sanphamctRepository.GetByIdAsync(sp.Idspct);
				var y = await _sanphamRepository.GetByIdAsync(x.Idsp);
				if (x != null)
				{
					x.Soluong -= sp.Soluong;
					if (x.Soluong == 0)
					{
						x.Trangthai = 1;
					}
					y.Soluong -= sp.Soluong;
					if(y.Soluong == 0)
					{
						y.Trangthai = 1;
					}
				}
				await _sanphamctRepository.UpdateAsync(x);
				await _sanphamRepository.UpdateAsync(y);
			}

			
		await _hoadonRepository.AddHoaDonWithDetails(hoadon, hoadonChiTiets);

			return hoadon;
		}
		public Task<List<Hoadon>> GetAllAsync()
		{
			return _hoadonRepository.GetAllAsync();
		}
		public async Task<Hoadon> AddHoaDonKhachhangthanthietoff(HoadonoffKhachhangthanthietDto dto, int diemSuDung)
		{
			// Tạo mới hóa đơn
			var hoadon = new Hoadon
			{
				Idnv = dto.Idnv ?? null,
				Idkh = dto.Idkh?? null,
				Idgg = dto.Idgg?? null,
				Idpttt = dto.Idpttt,

				Diachiship = "",
				Ngaygiaothucte = null,
				Sdt = dto.Sdt,
				Ghichu = dto.Ghichu,
				Thoigiandathang = DateTime.Now,
				Tongtiencantra = dto.Tongtiencantra,
				Tongtiensanpham = dto.Tongtiensanpham,
				Tonggiamgia = dto.Tonggiamgia,
				Trangthaidonhang = 3,
				Trangthaithanhtoan = 0,
					Trangthai = 1
			};

			// Thêm chi tiết hóa đơn
			var hoadonChiTiets = new List<Hoadonchitiet>();
			foreach (var sp in dto.SanPhamChiTiet)
			{
				var hoadonChiTiet = new Hoadonchitiet
				{
					Idspct = sp.Idspct,
					Soluong = sp.Soluong,
					Giasp = sp.Giasp,
					Giamgia = sp.Giamgia ?? 0
				};
				
				hoadonChiTiets.Add(hoadonChiTiet);
				var x = await _sanphamctRepository.GetByIdAsync(sp.Idspct);
				var y = await _sanphamRepository.GetByIdAsync(x.Idsp);
				if(x!= null)
				{
					x.Soluong -= sp.Soluong;
					if (x.Soluong == 0)
					{
						x.Trangthai = 1;
					}
					y.Soluong -= sp.Soluong;
					if (y.Soluong == 0)
					{
						y.Trangthai = 1;
					}
				}
				await _sanphamctRepository.UpdateAsync(x);
				await _sanphamRepository.UpdateAsync(y);

			}

		 await	_hoadonRepository.AddHoaDonWithDetails(hoadon, hoadonChiTiets);

			if (dto.Idgg.HasValue)
			{
				var giamgia = await _giamgiaRepository.GetByIdAsync(dto.Idgg.Value);
				if (giamgia != null && giamgia.Soluong > 0)
				{
					giamgia.Soluong -= 1;
				await	_giamgiaRepository.UpdateAsync(giamgia);
				}
			}

			// Cập nhật điểm sử dụng của khách hàng
			var khachhang = await _khachhangRepository.GetByIdAsyncThao(dto.Idkh.Value);
			if (khachhang != null)
			{
				// Trừ điểm đã sử dụng và cập nhật
				khachhang.Diemsudung = khachhang.Diemsudung-diemSuDung+(int)(dto.Tongtiencantra / 1000);
				khachhang.Tichdiem += dto.Tongtiencantra;
				var rank = await _rankRepository.GetAllRanksAsync();
				var rank1 = rank.Where(x=>x.Trangthai == 0).ToList();
				int rankkhonhtontai = 0;
				foreach (var r in rank1)
				{
					if(r.MinMoney<= khachhang.Tichdiem && r.MaxMoney>= khachhang.Tichdiem)
					{
						khachhang.Idrank = r.Id;
						rankkhonhtontai = 1;
					}
				}
				if (rankkhonhtontai == 0)
				{
					throw new Exception("Khách hàng có điểm tích không nằm trong các khoảng Rank. Vui lòng kiểm tra lại rank.");
				}
				else {
					await _khachhangRepository.UpdateAsyncThao(khachhang);
				}
			
			}

			return hoadon;
		}
		public async Task<IEnumerable<Hoadon>> GetAllHoadonsOlnAsync()
		{
			var a = await _hoadonRepository.GetAlOlnlAsync();
			return a;
		}
		public async Task<IEnumerable<Hoadon>> GetAllHoadonsAsync()
		{

			var a = await _hoadonRepository.GetAllAsync();

			
			return  a;
		}
		public async Task<IEnumerable<Hoadon>> GetAllOffHoadonsAsync()
		{

			var a = await _hoadonRepository.GetAllOffAsync();
			return a;
		}
		public List<Hoadon> GetAllHoadons()
		{
			var hoadons = _hoadonRepository.GetAllHoadons().ToList();

			
			return hoadons;
		}
		public async Task<Hoadon> GetByIdAsync(int id)
		{
			var hoadon = await _hoadonRepository.GetByIdAsync(id);

			return hoadon;
		}
		public async Task<(decimal TongTienThanhToan, int TongSoLuongDonHang)> GetDailyReportAsync(DateTime date)
		{
			return await _hoadonRepository.GetDailyReportAsync(date);
		}
		public IEnumerable<HoadonSummaryDto> GetOrderSummary(string timeUnit)
    {
        return _hoadonRepository.GetOrderSummaryByTime(timeUnit);
    }
		public async Task<HoadonupdatetrangthaiDto> RestoreStateAsync(int id, int trangthai)
		{
			var hoadon = await _hoadonRepository.GetByIdAsync(id);

			if (hoadon == null)
			{
				throw new Exception("Hóa đơn không tồn tại.");
			}

			// Gán trạng thái mới
			hoadon.Trangthai = trangthai;

			// Cập nhật hóa đơn
			await _hoadonRepository.UpdateAsync(hoadon);

			// Trả về DTO
			return new HoadonupdatetrangthaiDto
			{
				Id = hoadon.Id,
				Trangthai = hoadon.Trangthai
			};
		}
		public Task<int> GetSoDonAsync() => _hoadonRepository.CountDonHangAsync();
		public Task<decimal> GetDoanhThuThanhCongAsync() => _hoadonRepository.SumDoanhThuThanhCongAsync();

		public Task<List<LatestInvoiceDto>> Get10LatestInvoicesAsync()
		 => _hoadonRepository.Get10LatestInvoicesAsync();

	}
}
