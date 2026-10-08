using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IChatLieuService
    {
        Task<IEnumerable<ChatLieuDTO>> GetAllAsync();
        Task<ChatLieuDTO> GetByIdAsync(int id);
        Task<ChatLieuDTO> AddAsync(ChatLieuDTO dto);
        Task<ChatLieuDTO> UpdateAsync(int id, ChatLieuDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<List<ChatLieu>> GetListByIdsAsync();
    }
}
