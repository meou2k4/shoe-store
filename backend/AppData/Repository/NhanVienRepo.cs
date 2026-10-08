using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace AppData.Repository
{
    public class NhanVienRepo : INhanVienRepo
    {
        private readonly AppDbContext _context;
        public NhanVienRepo(AppDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<Nhanvien>> GetAllAsync()
        {
            return await _context.Set<Nhanvien>().ToListAsync();
        }

        public async Task<Nhanvien> GetByIdAsync(int id)
        {
            return await _context.Set<Nhanvien>().FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task AddAsync(Nhanvien kh)
        {
            await _context.nhanviens.AddAsync(kh);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Nhanvien nhanvien)
        {
            _context.Set<Nhanvien>().Update(nhanvien);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var nhanvien = await GetByIdAsync(id);
            if (nhanvien != null)
            {
                _context.Set<Nhanvien>().Remove(nhanvien);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tìm thấy nhân viên");
            }
        }

        public async Task<IEnumerable<Nhanvien>> TimKiemNhanvienAsync(string search)
        {
            if (search == null)
            {
                return await _context.nhanviens.ToListAsync();
            }
            else
            {
                search = search.ToLower();
                return await _context.nhanviens.Where(x => (x.Hovaten.StartsWith(search) || x.Sdt.StartsWith(search) || x.Diachi.StartsWith(search))&&(x.Trangthai==1||x.Trangthai==0)).ToListAsync();
            }
        }
		public async Task<int> CountNhanVienTrangThai0Async()
		{
			// Truy vấn trực tiếp bảng Nhanviens và đếm
			return await _context.nhanviens
								 .AsNoTracking()
								 .CountAsync(nv => nv.Trangthai == 0 && nv.Role == 1);
		}
		public async Task<Nhanvien?> GetByEmailAsync(string email)
		{
			return await _context.nhanviens
							.FirstOrDefaultAsync(u => u.Email == email);
		}
		public async Task<bool> ChangePasswordAsync(int id, string currentPwPlain, string newPwPlain)
		{
			var nv = await GetByIdAsync(id);
			if (nv == null) return false;

			// So khớp mật khẩu cũ
			var isMatch = BCrypt.Net.BCrypt.Verify(currentPwPlain, nv.Password);
			if (!isMatch) return false;

			// Hash & lưu mật khẩu mới
			nv.Password = BCrypt.Net.BCrypt.HashPassword(newPwPlain);
			nv.Ngaytaotaikhoan = DateTime.UtcNow;       // Ví dụ ghi log
			_context.nhanviens.Update(nv);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
