using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IDanhGiaService
    {
        Task<List<DanhgiaDTO>> GetAll();
        Task<DanhgiaDTO> GetById(int id);
        Task Create(DanhgiaDTO danhGiaDTO);
        Task Update(int id, DanhgiaDTO danhGiaDTO);
        Task Delete(int id);
        Task<DanhgiaDTO> getByidHDCT(int id);
        Task<List<DanhgiaDTO>> GetByidSP(int idsp);
    }
}
