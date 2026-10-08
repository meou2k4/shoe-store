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
    public class GioHangChiTetRepo : IGioHangChiTetRepo
    {
        private readonly AppDbContext _context;
        public GioHangChiTetRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Giohangchitiet ct)
        {
            if (_context.giohangs.Find(ct.Idgh) == null)
            {
                new Exception("Không tồn tại giỏ hàng");
            }
            else
            {
                if (_context.Sanphamchitiets.Find(ct.Idspct) == null)
                {
                    new Exception("Không tồn tại sản phẩm");
                }
                else
                {
                    if (ct.Soluong < 0)
                    {
                        new Exception("Vui lòng nhập số lượng lớn hơn hoặc bằng 0");
                    }
                    else
                    {
                        await _context.giohangchitiets.AddAsync(ct);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            var a = await GetByIdAsync(id);
            if (a != null)
            {
                _context.giohangchitiets.Remove(a);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tìm thấy nhân viên");
            }
        }

        public async Task<IEnumerable<Giohangchitiet>> GetAllAsync()
        {
            return await _context.giohangchitiets.ToListAsync();
        }

        public async Task<Giohangchitiet> GetByIdAsync(int id)
        {
            return await _context.giohangchitiets.FindAsync(id);
        }

        public async Task<Giohangchitiet> GetByIdspctToGiohangAsync(int idgh, int idspct)
        {
            return await _context.giohangchitiets
                .FirstOrDefaultAsync(x => x.Idgh == idgh && x.Idspct == idspct);
        }

        public async Task<List<Giohangchitiet>> GetGHCTByIdGH(int Idkh)
        {
            return await _context.giohangchitiets.Where(t => t.Idgh == Idkh).ToListAsync();
        }
        public async Task UpdateAsync(Giohangchitiet ct)
        {
            if (_context.giohangs.Find(ct.Idgh) == null)
            {
                new Exception("Không tồn tại giỏ hàng");
            }
            else
            {
                if (_context.Sanphamchitiets.Find(ct.Idspct) == null)
                {
                    new Exception("Không tồn tại sản phẩm chi tiết ");
                }
                else
                {
                    if (ct.Soluong < 0)
                    {
                        new Exception("Vui lòng nhập số lượng lớn hơn hoặc bằng 0");
                    }
                    else
                    {
                        _context.giohangchitiets.Update(ct);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
