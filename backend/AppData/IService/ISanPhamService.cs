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
    public interface ISanPhamService
    {
        Task<IEnumerable<SanphamDTO>> GetAllAsync();
        Task<SanphamDTO> GetByIdAsync(int id);
        Task AddAsync(SanphamDTO sanphamDto);
        Task UpdateAsync(int id, SanphamDTO sanphamDto);
        Task DeleteAsync(int id);
        Task UpdateStatusToCancelled(int id);
        Task UpdateStatusLoad(int id);
        Task<IEnumerable<SanphamDTO>> SearchByNameAsync(string name);
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamViewModels();
        Task<SanphamViewModel> GetAllSanphamViewModelsByIdSP(int idsp);
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamGiamGiaViewModels();
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamByThuongHieu(int id);
        Task<int> GetTotalSoldQuantityAsync(int idSanphamChitiet);
        Task<float> TinhTrungBinhDanhGia(int idSanpham);
        Task<IEnumerable<SanphamViewModel>> GetSanphamByThuocTinh(
        decimal? giaMin = null,
        decimal? giaMax = null,
         List<int> idThuongHieu = null,
     List<int> idSize = null, bool? coSale = null);
        Task<List<Sanpham>> GetListByIdsAsync(List<int> ids);
    }
}
