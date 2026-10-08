using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IThuongHieuRepo
    {
        Task<IEnumerable<Thuonghieu>> GetAllAsync();
        Task<Thuonghieu> GetByIdAsync(int id);
        Task<Thuonghieu> AddAsync(Thuonghieu entity);
        Task<Thuonghieu> UpdateAsync(Thuonghieu entity);
        Task<bool> DeleteAsync(int id);
    }
}
