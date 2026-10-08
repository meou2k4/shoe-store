using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IGioHangChiTetService
    {
        Task<IEnumerable<GiohangchitietDTO>> GetAllGiohangsAsync();
        Task<GiohangchitietDTO> GetGiohangByIdAsync(int id);
        Task<List<GiohangchitietDTO>> GetGHCTByIdGH(int Idkh);
        Task<GiohangchitietDTO> GetByIdspctToGiohangAsync(int idgh, int idspct);
        Task UpdateSoLuongGiohangAsync(int id, GiohangchitietDTO dto);
        Task AddGiohangAsync(GiohangchitietDTO dto);
        Task UpdateGiohangAsync(int id, GiohangchitietDTO dto);
        Task DeleteGiohangAsync(int id);
    }
}
