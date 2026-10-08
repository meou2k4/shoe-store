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
    public interface ITraHangChiTietService
    {
        Task<List<TrahangchitietDTO>> GetAll();
        Task<TrahangchitietDTO> GetById(int id);
        Task Add(TrahangchitietDTO ct);
        Task Update(int id, TrahangchitietDTO ct);
        Task<List<SanPhamTraHang>> SanphamByThct();
        Task Delete(int id);
        Task<List<HoadonchitietViewModel>> ListSanPhamByIdhd(int id);
        Task<List<TraHangchitietViewModel>> ListSanPhamByIdth(int id);
    }
}
