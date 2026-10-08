using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IRankService
    {
        Task<IEnumerable<RankDTO>> GetAllRanksAsync();
        Task<RankDTO> GetRankByIdAsync(int id);
        Task AddRankDTOAsync(RankDTO rankDto);
        Task UpdateRankAsync(int id, RankDTO rankDTO);
        Task DeleteRankAsync(int id);
    }
}
