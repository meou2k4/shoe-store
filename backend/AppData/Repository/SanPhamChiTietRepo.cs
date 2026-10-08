using AppData.DTO;
using AppData.IRepository;
using AppData.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class SanPhamChiTietRepo : ISanPhamChiTietRepo
    {
        private readonly AppDbContext _context;

        public SanPhamChiTietRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sanphamchitiet>> GetAllAsync()
        {
            try
            {
                return await _context.Sanphamchitiets.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm chi tiết: " + ex.Message);
            }
        }

        public async Task<Sanphamchitiet> GetByIdAsync(int id)
        {
            try
            {
                var result = await _context.Sanphamchitiets.FirstOrDefaultAsync(a => a.Id == id);
                if (result == null)
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm chi tiết với ID: " + id);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm sản phẩm chi tiết: " + ex.Message);
            }
        }
        public async Task<List<Sanphamchitiet>> GetByIdSPAsync(int idsp)
        {
            return await _context.Sanphamchitiets
                                   .Where(t => t.Idsp == idsp)
                                   .ToListAsync();
        }


        public async Task<Sanphamchitiet> AddAsync(Sanphamchitiet sanphamchitiet)
        {
            try
            {
                _context.Sanphamchitiets.Add(sanphamchitiet);
                await _context.SaveChangesAsync();
                return sanphamchitiet;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi thêm sản phẩm chi tiết: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Sanphamchitiet> UpdateAsync(Sanphamchitiet sanphamchitiet)
        {
            try
            {
                _context.Sanphamchitiets.Update(sanphamchitiet);
                await _context.SaveChangesAsync();
                return sanphamchitiet;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi cập nhật sản phẩm chi tiết: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var sanphamchitiet = await _context.Sanphamchitiets.FindAsync(id);
                if (sanphamchitiet == null)
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm chi tiết với ID: " + id);
                bool isReferenced = await _context.hoadonchitiets.AnyAsync(x => x.Idspct == id)
                                    || await _context.giohangchitiets.AnyAsync(x => x.Idspct == id)
                                    || await _context.salechitiets.AnyAsync(x => x.Idspct == id);
  

                if (isReferenced)
                {
                    sanphamchitiet.Soluong = 0;
                    sanphamchitiet.Trangthai = 3;
                    _context.Sanphamchitiets.Update(sanphamchitiet);
                }
                else
                {
                    _context.Sanphamchitiets.Remove(sanphamchitiet);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm chi tiết: " + ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<List<Sanphamchitiet>> GetListByIdsAsync(List<int> ids)
        {
            var spct = await _context.Sanphamchitiets
                                     .Include(h => h.Color)
                                     .Include(h => h.Size)
                                     .Include(h => h.ChatLieu)
                .Where(dg => ids.Contains(dg.Id))
                .ToListAsync();

            return spct;
        }
    }
}
