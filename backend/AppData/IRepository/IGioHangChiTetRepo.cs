using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IGioHangChiTetRepo
    {
        Task<IEnumerable<Giohangchitiet>> GetAllAsync();
        Task<Giohangchitiet> GetByIdAsync(int id);
        Task<Giohangchitiet> GetByIdspctToGiohangAsync(int idgh, int idspct);
        Task<List<Giohangchitiet>> GetGHCTByIdGH(int Idkh);
        Task AddAsync(Giohangchitiet ct);
        Task UpdateAsync(Giohangchitiet ct);
        Task DeleteAsync(int id);
    }
}
