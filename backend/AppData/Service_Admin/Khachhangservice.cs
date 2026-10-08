using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.IService;
using AppData.IService_Admin;
using AppData.Models;
using AppData.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace AppData.Service_Admin
{
	public class Khachhangservice:IKhackhangservice
	{
		private readonly Ikhachhangrepository _repository;
		private readonly IRankRepository _rankrepository;
		

		public Khachhangservice(Ikhachhangrepository repository, IRankRepository rankrepository)
		{
			_repository = repository;
			_rankrepository = rankrepository;


		}
		public async Task ToggleTrangthaiAsync(int Id)  // Thêm async
		{
			var rank = await _repository.GetByIdAsyncThao(Id);  // Lấy dữ liệu bất đồng bộ
			if (rank != null)
			{
				rank.Trangthai = (rank.Trangthai == 0) ? 1 : 0;  // Chuyển trạng thái
				await _repository.UpdateAsyncThao(rank);  // Cập nhật dữ liệu bất đồng bộ
			}
		}
		public async Task<KhachhangDto> GetByIdAsyncThao(int id)
		{
			var khachhang = await _repository.GetByIdAsyncThao(id);
			if (khachhang == null) throw new Exception("Khách hàng không tồn tại.");

			return new KhachhangDto
			{
				Ten = khachhang.Ten,
				Sdt = khachhang.Sdt,
				Ngaysinh = khachhang.Ngaysinh,
				Tichdiem = khachhang.Tichdiem,
				Email = khachhang.Email,
				Password = khachhang.Password,
				Ngaytaotaikhoan = khachhang.Ngaytaotaikhoan??DateTime.MinValue,
				Diemsudung = khachhang.Diemsudung,
				Trangthai = khachhang.Trangthai,
				Idrank = khachhang.Idrank
			};
		}

		// Lấy tất cả khách hàng
		public async Task<IEnumerable<Khachhang>> GetAllAsyncThao()
		{
			var khachhangs = await _repository.GetAllAsyncThao();
		
			return khachhangs.Select(kh => new Khachhang
			{
				Id = kh.Id,
				Ten = kh.Ten,
				Sdt = kh.Sdt,
				Ngaysinh = kh.Ngaysinh,
				Tichdiem = kh.Tichdiem,
				Email = kh.Email,
				Password = kh.Password,
				Ngaytaotaikhoan = kh.Ngaytaotaikhoan,
				Diemsudung = kh.Diemsudung,
				Trangthai = kh.Trangthai,
				
				Idrank = kh.Idrank
			});
		}

		// Tạo khách hàng mới
		public async Task CreateAsyncThao(KhachhangDto dto)
		{
			var khachhang = new Khachhang
			{
				Ten = dto.Ten,
				Sdt = dto.Sdt,
				Ngaysinh = dto.Ngaysinh,
				Tichdiem = 0,
				Email = dto.Email,
				Password = dto.Password,
				Ngaytaotaikhoan = DateTime.Now,
				Diemsudung = 0,
				Trangthai = 0,
				Idrank = dto.Idrank
			};
			var ranks = await _rankrepository.GetAllRanksAsync();
			var selectedRank = ranks.FirstOrDefault(r => khachhang.Tichdiem >= r.MinMoney && khachhang.Tichdiem <= r.MaxMoney && r.Trangthai ==0);

			if (selectedRank == null)
			{
				throw new Exception("Không tìm thấy Rank phù hợp với Tích điểm.");
			}

			khachhang.Idrank = selectedRank.Id;
			// Kiểm tra các điều kiện duy nhất trước khi tạo mới
			await _repository.CreateAsyncThao(khachhang);
			
		}

		// Cập nhật thông tin khách hàng
		public async Task UpdateAsyncThao(int id, KhachhangDto dto)
		{
			var existingKhachhang = await _repository.GetByIdAsyncThao(id);
			if (existingKhachhang == null) throw new Exception("Khách hàng không tồn tại.");

			// Cập nhật thông tin khách hàng
			existingKhachhang.Ten = dto.Ten;
			existingKhachhang.Sdt = dto.Sdt;
			existingKhachhang.Ngaysinh = dto.Ngaysinh;
			existingKhachhang.Tichdiem = dto.Tichdiem;
			existingKhachhang.Email = dto.Email;
			existingKhachhang.Password = dto.Password;
			existingKhachhang.Ngaytaotaikhoan = dto.Ngaytaotaikhoan;
			existingKhachhang.Diemsudung = dto.Diemsudung;
			existingKhachhang.Trangthai = dto.Trangthai;
			existingKhachhang.Idrank = dto.Idrank;

			await _repository.UpdateAsyncThao(existingKhachhang);
		}

		

		// Tìm kiếm khách hàng theo tên
		public async Task<IEnumerable<KhachhangDto>> SearchByNameAsyncThao(string name)
		{
			var khachhangs = await _repository.SearchByNameAsyncThao(name);
			return khachhangs.Select(kh => new KhachhangDto
			{
				Ten = kh.Ten,
				Sdt = kh.Sdt,
				Ngaysinh = kh.Ngaysinh,
				Tichdiem = kh.Tichdiem,
				Email = kh.Email,
				Password = kh.Password,
				Ngaytaotaikhoan = kh.Ngaytaotaikhoan ?? DateTime.MinValue,
				Diemsudung = kh.Diemsudung,
				Trangthai = kh.Trangthai,
				Idrank = kh.Idrank
			});
		}

		// Tìm kiếm khách hàng theo số điện thoại
		public async Task<IEnumerable<KhachhangDto>> SearchBySdtAsyncThao(string sdt)
		{
			var khachhangs = await _repository.SearchBySdtAsyncThao(sdt);
			return khachhangs.Select(kh => new KhachhangDto
			{
				Ten = kh.Ten,
				Sdt = kh.Sdt,
				Ngaysinh = kh.Ngaysinh,
				Tichdiem = kh.Tichdiem,
				Email = kh.Email,
				Password = kh.Password,
				Ngaytaotaikhoan = kh.Ngaytaotaikhoan ?? DateTime.MinValue,
				Diemsudung = kh.Diemsudung,
				Trangthai = kh.Trangthai,
				Idrank = kh.Idrank
			});
		}

		// Tìm kiếm khách hàng theo email
		public async Task<IEnumerable<KhachhangDto>> SearchByEmailAsyncThao(string email)
		{
			var khachhangs = await _repository.SearchByEmailAsyncThao(email);
			return khachhangs.Select(kh => new KhachhangDto
			{
				Ten = kh.Ten,
				Sdt = kh.Sdt,
				Ngaysinh = kh.Ngaysinh,
				Tichdiem = kh.Tichdiem,
				Email = kh.Email,
				Password = kh.Password,
				Ngaytaotaikhoan = kh.Ngaytaotaikhoan ?? DateTime.MinValue,
				Diemsudung = kh.Diemsudung,
				Trangthai = kh.Trangthai,
				Idrank = kh.Idrank
			});
		}
		public async Task<bool> DeleteKhachhangAsyncThao(int khachhangId)
		{
			
			var exists = await _repository.CheckForeignKeyConstraintAsync(khachhangId);

			var khachhang = await _repository.GetByIdAsyncThao(khachhangId);
			if (khachhang == null) return false; 

			if (exists)
			{
				khachhang.Trangthai = 2;
				await _repository.UpdateAsyncThao(khachhang);
				return true;
			}
			else
			{
				using (var transaction = await _repository.BeginTransactionAsync())
				{
					try
					{
						// Xóa các Danhgia liên quan
						await _repository.DeleteDanhgiasByKhachhangIdAsync(khachhangId);

						// Xóa Khachhang
						await _repository.DeleteAsyncThao(khachhang);

						// Commit transaction
						await transaction.CommitAsync();
					}
					catch
					{
						// Rollback transaction nếu có lỗi
						await transaction.RollbackAsync();
						throw;
					}
				}
			}
			return true;
		}

		public async Task<IEnumerable<Khachhang>> SearchByEmailTenSdtAsync(string keyword)
		{
			var khachhangs = await _repository.SearchByEmailTenSdtAsync(keyword);
			return khachhangs.Select(kh => new Khachhang
			{
				Id = kh.Id,
				Ten = kh.Ten,
				Sdt = kh.Sdt,
				Ngaysinh = kh.Ngaysinh,
				Tichdiem = kh.Tichdiem,
				Email = kh.Email,
				Password = kh.Password,
				Ngaytaotaikhoan = kh.Ngaytaotaikhoan,
				Diemsudung = kh.Diemsudung,
				Trangthai = kh.Trangthai,
				Idrank = kh.Idrank
			});
		}
	}
}
