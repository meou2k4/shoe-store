using AppData.Dto_Admin;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IRepository
{
    public interface IPhuongThucThanhToanRepo
    {
        Task<IEnumerable<Phuongthucthanhtoan>> GetAllAsync();
        Task<Phuongthucthanhtoan> GetByIdAsync(int id);
        Task AddAsync(Phuongthucthanhtoan entity);
        Task UpdateAsync(Phuongthucthanhtoan entity);
        Task DeleteAsync(int id);
		Task<List<PaymentMethodDTO>> GetActiveAsync();
	}
}
