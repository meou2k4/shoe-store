using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface ISanPhamChiTietService
    {
        Task<IEnumerable<Sanphamchitiet>> GetAllAsync();
        Task<SanphamchitietDTO> GetByIdAsync(int id);
        Task<List<SanphamchitietDTO>> GetByIdSPAsync(int idspct);
        Task AddAsync(SanphamchitietDTO dto);
        Task UpdateAsync(int id, SanphamchitietDTO dto);
        Task DeleteAsync(int id);
        Task<List<Sanphamchitiet>> GetListByIdsAsync(List<int> ids);
    }
}
