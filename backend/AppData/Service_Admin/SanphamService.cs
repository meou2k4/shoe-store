using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.IService_Admin;
using AppData.Models;

namespace AppData.Service_Admin
{
	public class SanphamService:ISanPhamservice
	{
		private readonly IsanphamRepos _repository;
        public SanphamService(IsanphamRepos repository)
        {
			_repository = repository;

		}
		public async Task<IEnumerable<Sanpham>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
			
		}
		public async Task<IEnumerable<SanphamDetailDto>> GetSanphamDetailsAsync()
		{
			return await _repository.GetSanphamDetailsAsync();
		}
		public async Task<List<ProductWithAttributesDTO>> GetAllActiveProductsWithAttributesAsync(int id)
		{
			return await _repository.GetAllActiveProductsWithAttributesAsync(id);
		}
		
		public async Task AddAsync(SanphamDTO sanphamDto)
		{
			var sanpham = new Sanpham
			{
				TenSanpham = sanphamDto.Tensp,
				Mota = sanphamDto.Mota,
				Soluong = 0,
				GiaBan = sanphamDto.Giaban,
				Idth = sanphamDto.Idth,
				NgayThemMoi=DateTime.Now,
				Trangthai = sanphamDto.Soluong > 0 ? 0 : 1
			};

			await _repository.AddAsync(sanpham);
		}
		public async Task<bool> AddSoluongAsync(int id, int soluongThem)
		{
			try
			{
				await _repository.AddSoluongAsync(id, soluongThem);
				return true;
			}
			catch (KeyNotFoundException)
			{
				
				return false;
			}
			catch (Exception)
			{
				throw; // Hoặc trả về false dựa trên chiến lược xử lý lỗi của bạn
			}
		}

		public async Task<bool> UpdateAsync(int id, SanphamDTO sanphamDto)
		{
			var sanpham = await _repository.GetByIdAsync(id);
			if (sanpham == null) return false;

			sanpham.TenSanpham = sanphamDto.Tensp;
			sanpham.Mota = sanphamDto.Mota;
			sanpham.Soluong = sanpham.Soluong;
			
		
			sanpham.GiaBan = sanphamDto.Giaban;

			sanpham.Idth = sanphamDto.Idth;
			sanpham.NgayThemMoi = sanpham.NgayThemMoi;
			await _repository.UpdateAsync(sanpham);
			return true;
		}

		public async Task DeleteAsync(int id) {
			
				await _repository.DeleteAsync(id);
			
		} 

		public async Task<IEnumerable<SanphamDTO>> SearchByNameAsync(string name)
		{
			var sanphams = await _repository.SearchByNameAsync(name);
			return sanphams.Select(sp => new SanphamDTO
			{
				Tensp = sp.TenSanpham,
				Mota = sp.Mota,
				Trangthai = sp.Trangthai,
				Soluong = sp.Soluong,
				Giaban = sp.GiaBan,
				Idth = sp.Idth,
				NgayThemMoi = sp.NgayThemMoi
			});
		}public async Task<IEnumerable<Sanpham>> SearchByNameHdAsync(string name)
		{
			var sanphams = await _repository.SearchByNameHdAsync(name);
			return sanphams.Select(sp => new Sanpham
			{
				Id = sp.Id,
				TenSanpham = sp.TenSanpham,
				
				Soluong = sp.Soluong,
				GiaBan = sp.GiaBan,
				Idth = sp.Idth,
				NgayThemMoi = sp.NgayThemMoi
			});
		}
		public async Task UpdateStatusLoad(int id)
		{

			var sale = await _repository.GetByIdAsync(id);
			if (sale == null)
			{
				throw new KeyNotFoundException("Sản phẩm không tồn tại");
			}
			
				if(sale.Trangthai == 2)
				{
					if (sale.Soluong > 0)
					{
						sale.Trangthai = 0; // Đang diễn ra
					}
					else if (sale.Soluong == 0)
					{
						sale.Trangthai = 1; // Chuẩn bị diễn ra
					}
				}
				if(sale.Trangthai == 0 || sale.Trangthai == 1)
				{
					sale.Trangthai = 2;
				}
				


			await _repository.UpdateAsync(sale);
		}
		public async Task UpdateStatusToCancelled(int id)
		{
			var sale = await _repository.GetByIdAsync(id);
			if (sale == null)
			{
				throw new KeyNotFoundException("Sản phẩm không tồn tại");
			}

			sale.Trangthai = 2; // Cập nhật trạng thái thành Hủy
			await _repository.UpdateAsync(sale);
		}
		public async Task<SanphamDTO> GetByIdAsync(int id)
		{
			var sanpham = await _repository.GetByIdAsync(id);

			if (sanpham == null)
				return null;
			return new SanphamDTO
			{
				Tensp = sanpham.TenSanpham,
				Mota = sanpham.Mota,
				Trangthai = sanpham.Trangthai,
				Soluong = sanpham.Soluong,
				Giaban = sanpham.GiaBan,
				Idth = sanpham.Idth,
				NgayThemMoi = sanpham.NgayThemMoi
			};
		}
		public Task<int> GetTongTrangThai0or1Async()
			=> _repository.CountTrangThai0or1Async();

	}
}
