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
    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepo _repos;
        private readonly IKhachHangRepo _KHrepos;
        public GioHangService(IGioHangRepo repos, IKhachHangRepo kHrepos)
        {
            _repos = repos;
            _KHrepos = kHrepos;
        }

        public async Task AddGiohangAsync(GiohangDTO dto)
        {
            // Kiểm tra xem khách hàng có tồn tại không
            var khachhang = await _KHrepos.GetByIdAsync(dto.Idkh);
            var idkh = await _repos.GetByIdKHAsync(dto.Idkh);
            if (khachhang == null) throw new ArgumentNullException("Khách hàng không tồn tại");
            else if (idkh != null) throw new ArgumentNullException("Khách hàng đã tồn tại trong sản phẩm");

            // Tạo đối tượng Hoadon từ DTO
            var gh = new Giohang()
            {
                Soluong = dto.Soluong,
                Idkh = dto.Idkh
            };

            // Thêm hóa đơn vào cơ sở dữ liệu
            await _repos.AddAsync(gh);
        }

        public async Task DeleteGiohangAsync(int id)
        {
            await _repos.DeleteAsync(id);
        }

        public async Task<IEnumerable<GiohangDTO>> GetAllGiohangsAsync()
        {
            var a = await _repos.GetAllAsync();
            return a.Select(g => new GiohangDTO()
            {
                Id = g.id,
                Soluong = g.Soluong,
                Idkh = g.Idkh
            });
        }

        public async Task<GiohangDTO> GetByIdKHAsync(int idspct)
        {
            try
            {
                var results = await _repos.GetByIdKHAsync(idspct);

                return new GiohangDTO()
                {
                    Id = results.id,
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm giỏ hàng khách hàng: " + ex.Message);
            }
        }

        public async Task<GiohangDTO> GetGiohangByIdAsync(int id)
        {
            var a = await _repos.GetByIdAsync(id);
            return new GiohangDTO()
            {
                Id = a.id,
                Soluong = a.Soluong,
                Idkh = a.Idkh
            };
        }

        public async Task UpdateGiohangAsync(int id, GiohangDTO dto)
        {
            var a = await _repos.GetByIdAsync(id);
            if (a == null) throw new KeyNotFoundException("Giỏ hàng không tồn tại.");
            a.Soluong = dto.Soluong;
            a.Idkh = dto.Idkh;
            await _repos.UpdateAsync(a);
        }
    }
}
