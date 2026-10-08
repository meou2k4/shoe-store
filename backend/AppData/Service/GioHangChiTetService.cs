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
    public class GioHangChiTetService : IGioHangChiTetService
    {
        private readonly IGioHangChiTetRepo _repos;
        public GioHangChiTetService(IGioHangChiTetRepo repos)
        {
            _repos = repos;
        }
        public async Task AddGiohangAsync(GiohangchitietDTO dto)
        {
            var gh = new Giohangchitiet()
            {
                Idgh = dto.Idgh,
                Idspct = dto.Idspct,
                Soluong = dto.Soluong
            };
            await _repos.AddAsync(gh);
        }

        public async Task DeleteGiohangAsync(int id)
        {
            await _repos.DeleteAsync(id);
        }

        public async Task<List<GiohangchitietDTO>> GetGHCTByIdGH(int idspct)
        {
            try
            {
                var results = await _repos.GetGHCTByIdGH(idspct);

                var dtoList = results.Select(result => new GiohangchitietDTO
                {
                    Id = result.Id,
                    Idgh = result.Idgh,
                    Idspct = result.Idspct,
                    Soluong = result.Soluong,
                }).ToList();

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm giỏ hàng chi tiết by giỏ hàng: " + ex.Message);
            }
        }

        public async Task<IEnumerable<GiohangchitietDTO>> GetAllGiohangsAsync()
        {
            var a = await _repos.GetAllAsync();
            return a.Select(x => new GiohangchitietDTO()
            {
                Id = x.Id,
                Idspct = x.Idspct,
                Idgh = x.Idgh,
                Soluong = x.Soluong
            });
        }

        public async Task<GiohangchitietDTO> GetByIdspctToGiohangAsync(int idgh, int idspct)
        {
            var entity = await _repos.GetByIdspctToGiohangAsync(idgh, idspct);
            if (entity == null) return null;

            // Mapping entity to DTO
            return new GiohangchitietDTO
            {
                Id = entity.Id,
                Idspct = entity.Idspct,
                Soluong = entity.Soluong,
                Idgh = entity.Idgh,
            };
        }

        public async Task<GiohangchitietDTO> GetGiohangByIdAsync(int id)
        {
            var a = await _repos.GetByIdAsync(id);
            return new GiohangchitietDTO()
            {
                Id = a.Id,
                Idspct = a.Idspct,
                Idgh = a.Idgh,
                Soluong = a.Soluong
            };
        }
        public async Task UpdateSoLuongGiohangAsync(int id, GiohangchitietDTO dto)
        {
            var a = await _repos.GetByIdAsync(id);
            if (a == null) throw new KeyNotFoundException("Giỏ hàng không tồn tại.");
            a.Soluong += dto.Soluong;
            a.Idgh = dto.Idgh;
            a.Idspct = dto.Idspct;
            await _repos.UpdateAsync(a);
        }

        public async Task UpdateGiohangAsync(int id, GiohangchitietDTO dto)
        {
            var a = await _repos.GetByIdAsync(id);
            if (a == null) throw new KeyNotFoundException("Giỏ hàng không tồn tại.");
            a.Soluong = dto.Soluong;
            a.Idgh = dto.Idgh;
            a.Idspct = dto.Idspct;
            await _repos.UpdateAsync(a);
        }
    }
}
