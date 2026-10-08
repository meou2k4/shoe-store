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
    public interface IHoaDonService
    {
        Task<IEnumerable<Hoadon>> GetAllAsync();
        Task UpdateTrangThaiAsync(int orderCode, int status, int trangthaiTT);
        Task UpdateTrangThaiChechOutAsync(int orderCode, int status, int trangthaiTT);

        Task<HoadonDTO> GetByIdAsync(int id);
        Task AddAsync(HoadonDTO dto);
        Task UpdateAsync(HoadonDTO dto, int id);
        Task DeleteAsync(int id);
        Task Danhandonhang(int id);
        Task<List<HoadonDTO>> Checkvoucher(int idspct);
        Task<List<HoadonDTO>> TimhoadontheoIdKH(int id, string? search);
    }
}
