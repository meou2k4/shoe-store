using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ISanPhamRepo
    {
        Task<IEnumerable<Sanpham>> GetAllAsync();
        Task<Sanpham> GetByIdAsync(int id);
        Task AddAsync(Sanpham sanpham);
        Task UpdateAsync(Sanpham sanpham);
        Task DeleteAsync(int id);
        Task<IEnumerable<Sanpham>> SearchByNameAsync(string name);
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamViewModels();
        Task<SanphamViewModel> GetSanphamViewModelByIdSP(int idsp);
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamGiamGiaViewModels();
        Task<IEnumerable<SanphamViewModel>> GetAllSanphamByThuongHieu(int id);
        Task<List<Sanpham>> GetListByIdsAsync(List<int> ids);
        Task<IEnumerable<SanphamViewModel>> GetSanphamByThuocTinh(

          decimal? giaMin = null,
          decimal? giaMax = null,
          int? idThuongHieu = null);

   
    }
}
