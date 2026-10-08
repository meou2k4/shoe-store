using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ISanPhamChiTietRepo
    {
        Task<IEnumerable<Sanphamchitiet>> GetAllAsync();
        Task<Sanphamchitiet> GetByIdAsync(int id);
        Task<List<Sanphamchitiet>> GetByIdSPAsync(int idsp);
        Task<Sanphamchitiet> AddAsync(Sanphamchitiet entity);
        Task<Sanphamchitiet> UpdateAsync(Sanphamchitiet entity);
        Task<List<Sanphamchitiet>> GetListByIdsAsync(List<int> ids);
        Task DeleteAsync(int id);
    }
}
