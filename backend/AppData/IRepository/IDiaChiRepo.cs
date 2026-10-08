using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IDiaChiRepo
    {
        Task<IEnumerable<Diachi>> GetAllDiaChi();
        Task<Diachi> GetByIdAsync(int id);
        Task<List<Diachi>> GetDiaChiByIdKH(int idsp);
        Task Create(Diachi diachi);
        Task Delete(int id);
        Task Update(Diachi diachi);
        Task SaveChanges();
    }
}
