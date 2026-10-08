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
    public class KhachHangRepo : IKhachHangRepo
    {
        private readonly AppDbContext _context;
        public KhachHangRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Khachhang kh)
        {
            await _context.khachhangs.AddAsync(kh);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var a = await GetByIdAsync(id);
            var khachhang = await _context.khachhangs.FindAsync(id);

            bool isReferenced = await _context.hoadons.AnyAsync(x => x.Idkh == id)
                || await _context.giohangs.AnyAsync(x => x.Idkh == id)
                || await _context.trahangs.AnyAsync(x => x.Idkh == id)
                || await _context.danhgias.AnyAsync(x => x.Idkh == id);

            if (isReferenced)
            {
                
                khachhang.Trangthai = 3;
                _context.khachhangs.Update(khachhang);
            }
            else
            {
                var diachis = _context.diachis.Where(d => d.Idkh == id);
                _context.diachis.RemoveRange(diachis); 

                if (a != null)
                {
                    _context.khachhangs.Remove(a);
                }
                else
                {
                    throw new KeyNotFoundException("Không tìm thấy Khách hàng");
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Khachhang>> GetAllAsync()
        {
            return await _context.khachhangs.ToListAsync();
        }

        public async Task<Khachhang> GetByEmailAsync(string email)
        {
            return await _context.khachhangs.FirstOrDefaultAsync(nv => nv.Email == email);
        }

        public async Task<Khachhang> GetByIdAsync(int id)
        {
            return await _context.khachhangs.FindAsync(id);
        }

        public async Task<IEnumerable<Khachhang>> TimKiemAsync(string search)
        {
            if (search == null)
            {
                return await _context.khachhangs.ToListAsync();
            }
            else
            {
                search = search.ToLower();
                return await _context.khachhangs.Where(x => x.Ten.StartsWith(search) || x.Sdt.StartsWith(search) || x.Email.StartsWith(search)).ToListAsync();
            }
        }

        public async Task UpdateAsync(Khachhang kh)
        {
            _context.khachhangs.Update(kh);
            await _context.SaveChangesAsync();
        }

    }
}
