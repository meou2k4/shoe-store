using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IRankRepo
    {
        Task<IEnumerable<Rank>> GetAllAsync();
        Task<Rank> GetByIdAsync(int id);
        Task AddAsync(Rank rank);
        Task UpdateAsync(Rank rank);
        Task DeleteAsync(int id);
    }
}
