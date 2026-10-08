using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IGiamGia_RankRepo
    {
        Task<IEnumerable<giamgia_rank>> GetAllAsync();
        Task<List<giamgia_rank>> GetByIdRankSPCTAsync(int idspct);
        Task<giamgia_rank> GetByIdAsync(int id);
        Task AddAsync(giamgia_rank giamgiarank);
        Task DeleteAsync(int idgiamgia, int idrank);
        Task DeletegiamgiaAsync(int idgiamgia);
    }
}
