using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ISaleChiTietRepo
    {
        Task<IEnumerable<Salechitiet>> GetAllAsync();
        Task<Salechitiet> GetByIdAsync(int id);
        Task<List<Salechitiet>> GetByIdAsyncSpct(int id);
        Task AddAsync(Salechitiet entity);
        Task UpdateAsync(Salechitiet entity);
        Task DeleteAsync(int id);
    }
}
