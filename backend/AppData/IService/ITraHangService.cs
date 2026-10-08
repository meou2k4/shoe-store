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
    public interface ITraHangService
    {
        Task<List<TrahangDTO>> GetAll();
        Task<TrahangDTO> GetById(int id);
        Task<TrahangDTO> GetById1(int id);
        Task Add(TrahangtaoDTO trahang);
        Task Update(int id, TrahangtaoDTO trahang);
        Task DeleteById(int id);
        Task Huydon(int id, int idnv, string? chuthich);
        Task UpdateTrangThaiHd(int id);  
        Task XacNhan(int id, string hinhthucxuly, int idnv, string? chuthich);
    }
}
