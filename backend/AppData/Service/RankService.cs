using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class RankService : IRankService
    {
        private readonly IRankRepo _repos;
        public RankService(IRankRepo repos)
        {
            _repos = repos;
        }
        public async Task AddRankDTOAsync(RankDTO rankDto)
        {
            var asf = new Rank()
            {
                Tenrank = rankDto.Tenrank,
                MaxMoney = rankDto.MaxMoney,
                MinMoney = rankDto.MinMoney,
                Trangthai = 0,

            };
            await _repos.AddAsync(asf);
        }

        public async Task DeleteRankAsync(int id)
        {
            await _repos.DeleteAsync(id);
        }

        public async Task<IEnumerable<RankDTO>> GetAllRanksAsync()
        {
            var a = await _repos.GetAllAsync();
            return a.Select(x => new RankDTO()
            {
                Id = x.Id,
                Tenrank = x.Tenrank,
                MaxMoney = x.MaxMoney,
                MinMoney = x.MinMoney,
                Trangthai = x.Trangthai,
            });
        }

        public async Task<RankDTO> GetRankByIdAsync(int id)
        {
            var x = await _repos.GetByIdAsync(id);
            return new RankDTO()
            {
                Tenrank = x.Tenrank,
                MaxMoney = x.MaxMoney,
                MinMoney = x.MinMoney,
                Trangthai = x.Trangthai,
            };
        }

        public async Task UpdateRankAsync(int id, RankDTO rankDTO)
        {
            var x = await _repos.GetByIdAsync(id);
            if (x == null) throw new KeyNotFoundException("Khách hàng không tồn tại.");
            x.Tenrank = rankDTO.Tenrank;
            x.MaxMoney = rankDTO.MaxMoney;
            x.MinMoney = rankDTO.MinMoney;
            x.Trangthai = rankDTO.Trangthai;
            await _repos.UpdateAsync(x);


        }
    }
}
