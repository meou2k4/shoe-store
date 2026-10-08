using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IGiamGiaRepo
    {
        Task<IEnumerable<Giamgia>> GetAllAsync();
        Task<Giamgia> GetByIdAsync(int id);
        Task<Giamgia> AddAsync(Giamgia giamgia);
        Task<Giamgia> UpdateAsync(Giamgia giamgia);
        Task AddRankToGiamgia(int giamgiaId, List<string> rankNames);
        Task DeleteAsync(Giamgia giamgia);
    }
}
