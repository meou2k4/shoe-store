using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IDiaChiService
    {
        Task<IEnumerable<DiachiDTO>> GetAllDiaChi();
        Task<Diachi> GetByIdAsync(int id);
        Task<List<DiachiDTO>> GetDiaChiByIdKH(int idsp);
        Task Create(DiachiDTO diachi);
        Task Delete(int id);
        Task Update(int id, DiachiDTO diaChiDTO);
    }
}
