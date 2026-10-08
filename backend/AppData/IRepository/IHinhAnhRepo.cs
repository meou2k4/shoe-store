using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IHinhAnhRepo
    {
        Task<IEnumerable<Hinhanh>> GetAllAsync();
        Task<List<Hinhanh>> GetByIdTraHangAsync(int id);
        Task<List<Hinhanh>> GetByIdDanhGiaAsync(int id);
        Task<Hinhanh> GetByIdAsync(int id);
        Task AddAsync(Hinhanh entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(Hinhanh entity);
    }
}
