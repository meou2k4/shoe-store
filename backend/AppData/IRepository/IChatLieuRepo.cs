using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IChatLieuRepo
    {
        Task<IEnumerable<ChatLieu>> GetAllAsync();
        Task<ChatLieu> GetByIdAsync(int id);
        Task<ChatLieu> AddAsync(ChatLieu entity);
        Task<ChatLieu> UpdateAsync(ChatLieu entity);
        Task<List<ChatLieu>> GetListByIdsAsync();
        Task<bool> DeleteAsync(int id);
    }
}
