using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IKhachHangRepo
    {
        Task<IEnumerable<Khachhang>> GetAllAsync();
        Task<Khachhang> GetByIdAsync(int id);
        Task AddAsync(Khachhang kh);
        Task UpdateAsync(Khachhang kh);
        Task DeleteAsync(int id);
        Task<IEnumerable<Khachhang>> TimKiemAsync(string search);

        Task<Khachhang> GetByEmailAsync(string email);
    }
}
