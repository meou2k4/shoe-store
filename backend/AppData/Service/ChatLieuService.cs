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
    public class ChatLieuService : IChatLieuService
    {
        private readonly IChatLieuRepo _repository;
        private readonly ISanPhamChiTietRepo _phamChiTietRepo;

        public ChatLieuService(IChatLieuRepo repository, ISanPhamChiTietRepo phamChiTietRepo)
        {
            _repository = repository;
            _phamChiTietRepo = phamChiTietRepo;
        }

        public async Task<IEnumerable<ChatLieuDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new ChatLieuDTO
            {
                Id = e.Id,
                Tenchatlieu = e.Tenchatlieu,
                Trangthai = e.Trangthai,
                IsUsedInProduct = IsChatLieuUsedInProduct(e.Id) // Kiểm tra xem chất liệu có được sử dụng trong sản phẩm không

            });
        }

        public async Task<ChatLieuDTO> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ChatLieuDTO
            {
                Id = entity.Id,
                Tenchatlieu = entity.Tenchatlieu,
                Trangthai = entity.Trangthai
            };
        }

        public async Task<ChatLieuDTO> AddAsync(ChatLieuDTO dto)
        {
            var addedEntity = await _repository.AddAsync(new Models.ChatLieu
            {
                Tenchatlieu = dto.Tenchatlieu,
                Trangthai = dto.Trangthai
            });

            return new ChatLieuDTO
            {
                Id = addedEntity.Id, // Lúc này Id đã được sinh ra từ DB
                Tenchatlieu = addedEntity.Tenchatlieu,
                Trangthai = addedEntity.Trangthai
            };
        }


        public async Task<ChatLieuDTO> UpdateAsync(int id, ChatLieuDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Tenchatlieu = dto.Tenchatlieu;
            entity.Trangthai = dto.Trangthai;

            var updatedEntity = await _repository.UpdateAsync(entity);
            return new ChatLieuDTO
            {
                Tenchatlieu = updatedEntity.Tenchatlieu,
                Trangthai = updatedEntity.Trangthai
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public bool IsChatLieuUsedInProduct(int chatlieuId)
        {
            var isUsed =  _phamChiTietRepo.GetAllAsync().Result
               .FirstOrDefault(spct => spct.IdChatLieu == chatlieuId);
            return isUsed != null;
        }
        public Task<List<ChatLieu>> GetListByIdsAsync()
        {
            return _repository.GetListByIdsAsync();
        }
    }
}
