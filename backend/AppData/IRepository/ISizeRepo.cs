using AppData.Models;
using AppData.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface ISizeRepo
    {
        Task<IEnumerable<Size>> GetAllAsync();
        Task<Size> GetByIdAsync(int id);
        Task<Size> AddAsync(Size entity);
        Task<Size> UpdateAsync(Size entity);
        Task<bool> DeleteAsync(int id);
        Task<List<Size>> GetListByIdsAsync();
    }
}
