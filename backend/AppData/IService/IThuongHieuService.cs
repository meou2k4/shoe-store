using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IThuongHieuService
    {
        Task<IEnumerable<ThuonghieuDTO>> GetAllAsync();
        Task<ThuonghieuDTO> GetByIdAsync(int id);
        Task<ThuonghieuDTO> AddAsync(ThuonghieuDTO dto);
        Task<ThuonghieuDTO> UpdateAsync(int id, ThuonghieuDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
