using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IGiamGiaService
    {
        Task<IEnumerable<GiamgiaDTO>> GetAllAsync();
        Task<GiamgiaDTO> GetByIdAsync(int id);
        Task AddAsync(GiamgiaDTOadd dto);
        Task UpdateAsync(int id, GiamgiaDTOadd dto);
        Task AddRankToGiamgia(GiamgiaDTO dto);
        Task DeleteAsync(int id);
    }
}
