using AppData.DTO;
using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ITraHangRepo
    {
        Task<List<TrahangDTO>> GetAll();
        Task<TrahangDTO> GetById1(int id);
        Task<Trahang> GetById(int id);
        Task Add(Trahang trhang);
        Task Update(Trahang trhang);
        Task DeleteById(int id);
        Task XacNhan(int id, string hinhthucxuly, int idnv, string? chuthich);
    }
}
