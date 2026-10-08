using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class GiamGia_RankService : IGiamGia_RankService
    {
        private readonly IGiamGia_RankRepo _repository;
        private readonly IRankRepo _Rankrepository;
        private readonly IGiamGiaRepo _GGrepository;
        public GiamGia_RankService(IGiamGia_RankRepo repository, IRankRepo Rankrepository, IGiamGiaRepo GGrepository)
        {
            _repository = repository;
            _Rankrepository = Rankrepository;
            _GGrepository = GGrepository;
        }

        public async Task<IEnumerable<giamgia_rank>> GetAllAsync()
        {

            var entities = await _repository.GetAllAsync();

            return entities.Select(hoaDon => new giamgia_rank
            {
                IDgiamgia = hoaDon.IDgiamgia,
                Idrank = hoaDon.Idrank,
            });
        }

        public async Task<giamgia_rank> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new giamgia_rank
            {
                IDgiamgia = entity.IDgiamgia,
                Idrank = entity.Idrank,
            };
        }

        public async Task<List<giamgia_rankDTO>> GetByIdRankSPCTAsync(int idspct)
        {
            try
            {
                // Gọi repository để lấy dữ liệu
                var results = await _repository.GetByIdRankSPCTAsync(idspct);

                if (results == null || !results.Any())
                    throw new KeyNotFoundException("Không tìm thấy Sale-rank chi tiết với ID: " + idspct);

                // Ánh xạ thủ công từ entity sang DTO
                var dtoList = results.Select(result => new giamgia_rankDTO
                {
                    Idrank = result.Idrank,
                    IDgiamgia = result.IDgiamgia,
                }).ToList();

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm thuộc tính sản phẩm chi tiết: " + ex.Message);
            }
        }

        public async Task AddAsync(giamgia_rankDTO hoaDonDTO)
        {
            var rank = await _Rankrepository.GetByIdAsync(hoaDonDTO.Idrank);
            if (rank == null) throw new ArgumentNullException("Rank không tồn tại");


            var giamgia = await _GGrepository.GetByIdAsync(hoaDonDTO.IDgiamgia);
            if (giamgia == null) throw new ArgumentNullException("Giảm giá không tồn tại");

            // Tạo đối tượng Hoadon từ DTO
            var hoaDon = new giamgia_rank
            {
                IDgiamgia = hoaDonDTO.IDgiamgia,
                Idrank = hoaDonDTO.Idrank,
            };

            // Thêm hóa đơn vào cơ sở dữ liệu
            await _repository.AddAsync(hoaDon);
        }

        public async Task DeleteAsync(int idgiamgia, int idrank)
        {
            await _repository.DeleteAsync(idgiamgia, idrank);
        }
        public async Task DeletegiamgiaAsync(int idgiamgia)
        {
            await _repository.DeletegiamgiaAsync(idgiamgia);
        }
    }
}
