using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface ISaleChiTietService
    {
        Task<IEnumerable<Salechitiet>> GetAllAsync();
        Task<Salechitiet> GetByIdAsync(int id);
        Task<Salechitiet> GetByIdAsyncSpct(int id);
        Task AddAsync(SalechitietDTO entity);
        Task UpdateAsync(SalechitietDTO entity, int id);
        Task DeleteAsync(int id);
    }
}
