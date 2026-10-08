using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface ISizeService
    {
        Task<IEnumerable<SizeDTO>> GetAllAsync();
        Task<SizeDTO> GetByIdAsync(int id);
        Task<SizeDTO> AddAsync(SizeDTO dto);
        Task<SizeDTO> UpdateAsync(int id, SizeDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<List<Models.Size>> GetListByIdsAsync();
    }
}
