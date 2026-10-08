using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IGioHangService
    {
        Task<IEnumerable<GiohangDTO>> GetAllGiohangsAsync();
        Task<GiohangDTO> GetGiohangByIdAsync(int id);
        Task<GiohangDTO> GetByIdKHAsync(int id);
        Task AddGiohangAsync(GiohangDTO dto);
        Task UpdateGiohangAsync(int id, GiohangDTO dto);
        Task DeleteGiohangAsync(int id);
    }
}
