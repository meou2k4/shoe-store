using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface INhanVienRepo
    {
        Task<IEnumerable<Nhanvien>> GetAllAsync();
        Task<Nhanvien> GetByIdAsync(int id);
        Task AddAsync(Nhanvien nhanvien);
        Task UpdateAsync(Nhanvien nhanvien);
        Task DeleteAsync(int id); 
        Task<IEnumerable<Nhanvien>> TimKiemNhanvienAsync(string search);
        Task<int> CountNhanVienTrangThai0Async();
		Task<bool> ChangePasswordAsync(int id, string currentPwPlain, string newPwPlain);
        Task<Nhanvien?> GetByEmailAsync(string email);
	}
}
