using AppData.DTO;
using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IHoaDonChiTietService
    {
        Task<IEnumerable<Hoadonchitiet>> GetAllAsync();
        Task<Hoadonchitiet> GetByIdAsync(int id);
        Task AddAsync(HoadonchitietDTO dto);
        Task ReturnProductAsync(int hoadonId);
        Task UpdateAsync(HoadonchitietDTO dto, int id);
        Task DeleteAsync(int id);
        Task<List<HoadonchitietViewModel>> HoadonchitietTheoMaHD(int id);
        Task<List<HoadonchitietViewModel>> HoadonchitietTheoMaHD1(int id);
        Task<List<HoadonchitietViewModel>> Checksoluong(int id);
    }
}
