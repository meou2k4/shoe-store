using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class GioHangRepo : IGioHangRepo
    {
        private readonly AppDbContext _context;
        public GioHangRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Giohang gh)
        {
            try
            {
                await _context.giohangs.AddAsync(gh);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Lỗi khi thêm giỏ hàng vào cơ sở dữ liệu.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi thêm giỏ hàng.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var a = await GetByIdAsync(id);
            if (a != null)
            {
                _context.giohangs.Remove(a);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tìm thấy nhân viên");
            }
        }

        public async Task<IEnumerable<Giohang>> GetAllAsync()
        {
            return await _context.giohangs.ToListAsync();
        }

        public async Task<Giohang> GetByIdAsync(int id)
        {
            return await _context.giohangs.FindAsync(id);
        }

        public async Task<Giohang> GetByIdKHAsync(int id)
        {
            return await _context.giohangs
                                 .Include(h => h.Khachhang)
                                 .FirstOrDefaultAsync(h => h.Idkh == id);
        }

        public async Task UpdateAsync(Giohang gh)
        {
            _context.giohangs.Update(gh);
            await _context.SaveChangesAsync();
        }
    }
}
