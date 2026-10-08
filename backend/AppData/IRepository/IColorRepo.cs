using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IColorRepo
    {
        Task<IEnumerable<Color>> GetAllAsync();
        Task<Color> GetByIdAsync(int id);
        Task<Color> AddAsync(Color entity);
        Task<Color> UpdateAsync(Color entity);
        Task<bool> DeleteAsync(int id);
        Task<List<int>> GetUniqueColorsByProductIdAsync(int productId);
        Task<List<Color>> GetListByIdsAsync();
    }
}
