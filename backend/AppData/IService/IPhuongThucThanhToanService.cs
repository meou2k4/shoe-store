using AppData.DTO;
using AppData.Dto_Admin;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IPhuongThucThanhToanService
    {
        Task<IEnumerable<PhuongthucthanhtoanDTO>> GetAllAsync();
        Task<PhuongthucthanhtoanDTO> GetByIdAsync(int id);
        Task AddAsync(PhuongthucthanhtoanDTO dto);
        Task UpdateAsync(int id, PhuongthucthanhtoanDTO dto);
        Task DeleteAsync(int id);
		Task<List<PaymentMethodDTO>> GetActiveAsync();
	}
}
