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
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppData.Service_Admin
{
    public class GiamgiaService: IGiamgiaService
	{
		
		private readonly IGiamgiaRepos _repository;
		private readonly IMapper _mapper;
		private readonly IRankRepository _rankRepository;
		private readonly AppDbContext _context;
        public GiamgiaService(IGiamgiaRepos repository, IMapper mapper, IRankRepository rankRepository,AppDbContext context)
		{
			
			_repository = repository;
			_mapper = mapper;
			_rankRepository = rankRepository;
			_context = context;

		}

		

		public async Task<IEnumerable<Giamgia>> GetAllAsync()
		{
			var giamgias = await _repository.GetAllAsync();
			return _mapper.Map<IEnumerable<Giamgia>>(giamgias);
		}

		public async Task<GiamgiaDTO> GetByIdAsync(int id)
		{
			var giamgia = await _repository.GetByIdAsync(id);
			if (giamgia == null) throw new KeyNotFoundException("Không tìm thấy mã giảm giá.");

			return _mapper.Map<GiamgiaDTO>(giamgia);
		}

		public async Task AddAsync(GiamgiaDTO dto)
		{
			var giamgia = _mapper.Map<Giamgia>(dto);
			await _repository.AddAsync(giamgia);
			
		}
		public async Task<List<Giamgia>> SearchByDescriptionAsync(string description)
		{
			return await _repository.SearchByDescriptionAsync(description);
		}


		public async Task ChangeTrangthaiAsync(int id)
		{
			var giamgia = await _repository.GetByIdAsync(id);
			if (giamgia == null)
			{
				throw new KeyNotFoundException("Giảm giá không tồn tại");
			}

			// Nếu trạng thái là 0, 1, hoặc 3, chuyển sang trạng thái 2
			if (giamgia.Trangthai == 0 || giamgia.Trangthai == 1 || giamgia.Trangthai == 3)
			{
				giamgia.Trangthai = 2;  // Chuyển về trạng thái "dừng phát hành"
			}
			else if (giamgia.Trangthai == 2)
			{
				var currentDate = DateTime.Now;

				if (giamgia.Ngaybatdau > currentDate)
				{
					giamgia.Trangthai = 1;  // Chuyển trạng thái "chuẩn bị phát hành"
				}
				else if (giamgia.Ngaybatdau <= currentDate && giamgia.Ngayketthuc >= currentDate)
				{
					giamgia.Trangthai = 0;  // Chuyển trạng thái "đang phát hành"
				}
				else if (giamgia.Ngayketthuc < currentDate)
				{
					giamgia.Trangthai = 3;  // Chuyển trạng thái "đã phát hành"
				}
			}

			await _repository.UpdateAsync(giamgia);
			await _context.SaveChangesAsync();
			
		
		}
		public async Task UpdateAsync(int id, GiamgiaDTO dto)
		{
			var existingGiamgia = await _repository.GetByIdAsync(id);
			if (existingGiamgia == null) throw new KeyNotFoundException("Không tìm thấy mã giảm giá.");
			_mapper.Map(dto, existingGiamgia);
			await _repository.UpdateAsync(existingGiamgia);
			
		}


		public async Task UpdateTrangthaiContinuouslyAsync()
		{
			
				var currentDate = DateTime.Now;

				// Lấy danh sách tất cả các bản ghi Giamgia
				var giamgias = await _repository.GetAllAsync();

				var giamgia1 = giamgias.Where(x => x.Trangthai == 0 || x.Trangthai == 1 || x.Trangthai == 3);
				foreach (var giamgia in giamgia1)
				{
					if (giamgia.Ngaybatdau <= currentDate && giamgia.Ngayketthuc >= currentDate)
					{
						giamgia.Trangthai = 0; 
					}
					else if (giamgia.Ngayketthuc < currentDate )
					{
						giamgia.Trangthai = 3; // Đã phát hành
					}
					else if (giamgia.Ngaybatdau > currentDate)
					{
						giamgia.Trangthai = 1; // Chuẩn bị phát hành
					}
				}

				// Cập nhật trạng thái cho tất cả các bản ghi
				await _repository.UpdateRangeAsync(giamgias);
				await Task.Delay(10000); // Đợi 10 giây trước khi kiểm tra lại
			
		}
		
		public async Task<bool> DeleteGiamgiaAsync(int giamgiaId)
		{
			var giamgia = await _repository.GetByIdAsync(giamgiaId);
			if (await _repository.DeleteGiamgiaAsync(giamgiaId) == true)
			{
				return await _repository.DeleteGiamgiaAsync(giamgiaId);
			}
			else
			{
				giamgia.Trangthai = 4;
			  await	_repository.UpdateAsync(giamgia);
				return true;
			}
			
		}
		public async Task<IEnumerable<Giamgia>> GetVouchersByCustomerIdAsync(int customerId)
		{
			var vouchers = await _repository.GetVouchersByCustomerIdAsync(customerId);
			return vouchers.Select(g => new Giamgia
			{
				Id = g.Id,
				Mota = g.Mota,
				Donvi = g.Donvi,
				Soluong = g.Soluong,
				Giatri = g.Giatri,
				Ngaybatdau = g.Ngaybatdau,
				Ngayketthuc = g.Ngayketthuc,
				Trangthai = g.Trangthai
			});
		}

		public async Task AddRankToGiamgia(Giamgia_RankDTO dto)
		{

			if (dto.Donvi == 1)
			{
				if (dto.Giatri <= 0 || dto.Giatri > 100)
				{
					throw new ArgumentException("Giá trị phải lớn hơn 0 và nhỏ hơn hoặc bằng 100 khi Donvi là 0.");
				}
			}
			if (dto.Donvi == 0)
			{
				if (dto.Giatri <= 0)
				{
					throw new ArgumentException("Giá trị phải lớn hơn 0 khi Donvi là VND.");
				}
			}

			var currentDate = DateTime.Now;

			if (dto.Ngaybatdau > currentDate)
			{
				dto.Trangthai = 1;  // Chuyển trạng thái "chuẩn bị phát hành"
			}
			else if (dto.Ngaybatdau <= currentDate && dto.Ngayketthuc >= currentDate)
			{
				dto.Trangthai = 0;  // Chuyển trạng thái "đang phát hành"
			}
			else if (dto.Ngayketthuc < currentDate)
			{
				dto.Trangthai = 3;  // Chuyển trạng thái "đã phát hành"
			}
			var giamgia = new Giamgia
			{
				Mota = dto.Mota,
				Donvi = dto.Donvi,
				Giatri = dto.Giatri,
				Soluong =dto.Soluong,	
				Ngaybatdau = dto.Ngaybatdau,
				Ngayketthuc = dto.Ngayketthuc,
				Trangthai = dto.Trangthai
			};

			await _repository.AddAsync(giamgia);
			await _repository.AddRankToGiamgia(giamgia.Id, dto.RankNames);
		}

		public Task<bool> IsValidDateRange(DateTime ngaybatdau, DateTime ngayketthuc)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateGiamgiaRankAsync(int id, Giamgia_RankDTO dto)
		{
			var currentDate = DateTime.Now;

			var giamgia = await _repository.GetByIdAsync(id);
			if (giamgia == null)
			{
				throw new KeyNotFoundException("Không tìm thấy giảm giá với ID này.");
			}

			if (giamgia.Trangthai == 2)
			{
				dto.Trangthai = 2;
			}
			else
			{
				if (dto.Ngaybatdau <= currentDate && dto.Ngayketthuc >= currentDate)
				{
					dto.Trangthai = 0;
				}
				else if (dto.Ngayketthuc < currentDate)
				{
					dto.Trangthai = 3; // Đã phát hành
				}
				else if (dto.Ngaybatdau > currentDate)
				{
					dto.Trangthai = 1; // Chuẩn bị phát hành
				}
			}

			// Cập nhật các thuộc tính của Giamgia
			giamgia.Mota = dto.Mota;
			giamgia.Donvi = dto.Donvi;
			giamgia.Soluong = dto.Soluong;
			giamgia.Giatri = dto.Giatri;
			giamgia.Ngaybatdau = dto.Ngaybatdau;
			giamgia.Ngayketthuc = dto.Ngayketthuc;
			giamgia.Trangthai = dto.Trangthai;
			



			

			// Gọi UpdateAsync sau khi đã thực hiện tất cả các thay đổi
			await _repository.UpdateAsync(giamgia);
			var itemsToRemove = _context.giamgia_Ranks.Where(x => x.IDgiamgia == giamgia.Id).ToList();
			_context.giamgia_Ranks.RemoveRange(itemsToRemove);
			await _context.SaveChangesAsync();

			await _repository.AddRankToGiamgia(giamgia.Id, dto.RankNames);

		}
	}
}

