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
    public class HinhAnhService : IHinhAnhService
    {
        private readonly IHinhAnhRepo _repository;
        private readonly ITraHangRepo _THrepository;
        private readonly IDanhGiaRepo _DGrepository;
        public HinhAnhService(IHinhAnhRepo repository, ITraHangRepo THrepository, IDanhGiaRepo DGrepository)
        {
            _repository = repository;
            _THrepository = THrepository;
            _DGrepository = DGrepository;
        }

        public async Task AddAsync(HinhanhDTO entity)
        {
            // Kiểm tra nếu trà hàng không tồn tại
            if(entity.Idtrahang != 0)
            {
                int idtrahang = (int)entity.Idtrahang;
                var trahang = await _THrepository.GetById(idtrahang);
                if (trahang == null)
                    throw new ArgumentNullException("Trà hàng không tồn tại");
            }

            if (entity.Iddanhgia != 0)
            {
                int iddanhgia = (int)entity.Iddanhgia;
                var danhgia = await _DGrepository.GetById(iddanhgia);
                if (danhgia == null)
                    throw new ArgumentNullException("Đánh giá không tồn tại");
            }
            // Tạo đối tượng Hinhanh mới
            var hinhanh = new Hinhanh
            {
                Urlhinhanh = entity.Urlhinhanh,
                Idtrahang = entity.Idtrahang == 0 ? (int?)null : entity.Idtrahang,
                Iddanhgia = entity.Iddanhgia == 0 ? (int?)null : entity.Iddanhgia
            };

            // Thêm hình ảnh vào cơ sở dữ liệu và đợi kết quả
            await _repository.AddAsync(hinhanh);

            // Trả về dữ liệu hình ảnh đã thêm vào cơ sở dữ liệu
            entity.Id = hinhanh.Id; // Gán ID từ Hoadon vào DTO
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Hinhanh>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(hoaDon => new Hinhanh
            {
                Id = hoaDon.Id,
                Idtrahang = hoaDon.Idtrahang,
                Iddanhgia = hoaDon.Iddanhgia,
                Urlhinhanh = hoaDon.Urlhinhanh,
            });
        }

        public async Task<Hinhanh> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new Hinhanh
            {
                Id = entity.Id,
                Idtrahang = entity.Idtrahang,
                Iddanhgia = entity.Iddanhgia,
                Urlhinhanh = entity.Urlhinhanh,
            };
        }


        public async Task<List<Hinhanh>> GetByIdTraHangAsync(int id)
        {
            var entities = await _repository.GetByIdTraHangAsync(id);

            if (entities == null || !entities.Any())
                return new List<Hinhanh>();

            return entities.Select(entity => new Hinhanh
            {
                Id = entity.Id,
                Idtrahang = entity.Idtrahang,
                Iddanhgia = entity.Iddanhgia,
                Urlhinhanh = entity.Urlhinhanh,
            }).ToList();
        }

        public async Task<List<Hinhanh>> GetByIdDanhGiaAsync(int id)
        {
            var entities = await _repository.GetByIdDanhGiaAsync(id);

            if (entities == null || !entities.Any())
                return new List<Hinhanh>();

            return entities.Select(entity => new Hinhanh
            {
                Id = entity.Id,
                Idtrahang = entity.Idtrahang,
                Iddanhgia = entity.Iddanhgia,
                Urlhinhanh = entity.Urlhinhanh,
            }).ToList();
        }

        public async Task UpdateAsync(HinhanhDTO dto, int id)
        {

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Hình ảnh không tồn tại");

            if (entity.Idtrahang != null)
            {
                int idtrahang = (int)entity.Idtrahang;
                var trahang = await _THrepository.GetById(idtrahang);
                if (trahang == null)
                    throw new ArgumentNullException("Trà hàng không tồn tại");
            }

            if (entity.Idtrahang != null)
            {
                int iddanhgia = (int)entity.Iddanhgia;
                var danhgia = await _DGrepository.GetById(iddanhgia);
                if (danhgia == null)
                    throw new ArgumentNullException("Đánh giá không tồn tại");
            }

            if (entity != null)
            {
                entity.Idtrahang = dto.Idtrahang;
                entity.Iddanhgia = dto.Iddanhgia;
                entity.Urlhinhanh = dto.Urlhinhanh;

                await _repository.AddAsync(entity);
            }
            ;

        }
    }
}
