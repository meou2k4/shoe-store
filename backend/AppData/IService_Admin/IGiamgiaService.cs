using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IService_Admin
{
    public interface IGiamgiaService
    {
		Task UpdateTrangthaiContinuouslyAsync();
		Task<IEnumerable<Giamgia>> GetAllAsync();
        Task<GiamgiaDTO> GetByIdAsync(int id);
        Task AddAsync(GiamgiaDTO dto);
        Task UpdateAsync(int id, GiamgiaDTO dto);
        Task AddRankToGiamgia(Giamgia_RankDTO dto);
		Task<bool> DeleteGiamgiaAsync(int giamgiaId);
		Task<List<Giamgia>> SearchByDescriptionAsync(string description);
		Task ChangeTrangthaiAsync(int id);
		Task UpdateGiamgiaRankAsync(int id, Giamgia_RankDTO dto);
		Task<IEnumerable<Giamgia>> GetVouchersByCustomerIdAsync(int customerId);
	}
}
