using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ITraHangChiTietRepo
    {
        Task<List<Trahangchitiet>> GetAll();
        Task<Trahangchitiet> GetById(int id);
        Task Add(Trahangchitiet ct);
        Task Update(Trahangchitiet ct);
        Task Delete(int id);
        Task<List<SanPhamTraHang>> SanphamByThct();
        Task< List<HoadonchitietViewModel>> ListSanPhamByIdhd(int id);
        Task<List<TraHangchitietViewModel>> ListSanPhamByIdth(int id);
    }
}
