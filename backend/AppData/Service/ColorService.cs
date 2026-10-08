using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class ColorService : IColorService
    {
        private readonly IColorRepo _repository;
        private readonly ISanPhamChiTietRepo _phamChiTietRepo;

        public ColorService(IColorRepo repository, ISanPhamChiTietRepo phamChiTietRepo)
        {
            _repository = repository;
            _phamChiTietRepo = phamChiTietRepo;
        }

        public async Task<IEnumerable<ColorDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new ColorDTO
            {
                Id = e.Id,
                Tenmau = e.Tenmau,
                Mamau = e.Mamau,
                Trangthai = e.Trangthai,
                IsUsedInProduct = IsColorUsedInProduct(e.Id) 

            });
        }

        public async Task<ColorDTO> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ColorDTO
            {
                Id = entity.Id,
                Tenmau = entity.Tenmau,
                Mamau = entity.Mamau,
                Trangthai = entity.Trangthai
            };
        }

        public async Task<ColorDTO> AddAsync(ColorDTO dto)
        {
            var entity = new Models.Color
            {
                Tenmau = dto.Tenmau,
                Mamau = dto.Mamau,
                Trangthai = dto.Trangthai
            };

            var addedEntity = await _repository.AddAsync(entity);
            return new ColorDTO
            {
                Id = addedEntity.Id,
                Tenmau = addedEntity.Tenmau,
                Mamau = addedEntity.Mamau,
                Trangthai = addedEntity.Trangthai
            };
        }
        public async Task<List<int>> GetColorsForProductAsync(int productId)
        {
            return await _repository.GetUniqueColorsByProductIdAsync(productId);
        }

        public async Task<ColorDTO> UpdateAsync(int id, ColorDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Tenmau = dto.Tenmau;
            entity.Mamau = dto.Mamau;
            entity.Trangthai = dto.Trangthai;

            var updatedEntity = await _repository.UpdateAsync(entity);
            return new ColorDTO
            {
                Tenmau = updatedEntity.Tenmau,
                Mamau = updatedEntity.Mamau,
                Trangthai = updatedEntity.Trangthai
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
        public bool IsColorUsedInProduct(int colorId)
        {
            // Kiểm tra nếu có sản phẩm nào sử dụng màu này
            var productWithColor = _phamChiTietRepo.GetAllAsync()
                .Result
                .FirstOrDefault(p => p.IdMau == colorId); // Kiểm tra nếu sản phẩm có ColorId là colorId

            return productWithColor != null; // Nếu có sản phẩm sử dụng màu này, trả về true
        }

        public Task<List<Color>> GetListByIdsAsync()
        {
            return _repository.GetListByIdsAsync();
        }
    }
}
