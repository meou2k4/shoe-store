using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IHoaDonRepo
    {
        Task<IEnumerable<Hoadon>> GetAllAsync();
        Task<Hoadon> GetByIdAsync(int id);
        Task AddAsync(Hoadon entity);
        Task UpdateAsync(Hoadon entity);
        Task DeleteAsync(int id);
        Task<List<Hoadon>> Checkvoucher(int idspct);
        Task<List<Hoadon>> TimhoadontheoIdKH(int id, string? search);
        Task Danhandonhang(int id);
    }
}
