using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IHinhAnhService
    {
        Task<IEnumerable<Hinhanh>> GetAllAsync();
        Task<List<Hinhanh>> GetByIdDanhGiaAsync(int id);
        Task<List<Hinhanh>> GetByIdTraHangAsync(int id);
        Task<Hinhanh> GetByIdAsync(int id);
        Task AddAsync(HinhanhDTO entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(HinhanhDTO entity, int id);
    }
}
