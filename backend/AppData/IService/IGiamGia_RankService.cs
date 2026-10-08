using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IGiamGia_RankService
    {
        Task<IEnumerable<giamgia_rank>> GetAllAsync();
        Task<giamgia_rank> GetByIdAsync(int id);
        Task<List<giamgia_rankDTO>> GetByIdRankSPCTAsync(int idspct);
        Task AddAsync(giamgia_rankDTO dto);
        Task DeleteAsync(int idgiamgia, int idrank);
        Task DeletegiamgiaAsync(int idgiamgia);
    }
}
