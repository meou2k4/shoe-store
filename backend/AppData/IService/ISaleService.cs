using AppData.DTO;
using AppData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface ISaleService
    {
        Task<IEnumerable<Sale>> GetAllWithIdAsync();
        Task<SaleDTO> GetByIdAsync(int id);
        Task AddAsync(SaleDTO saleDto);
        Task UpdateStatusToCancelled(int id);
        Task UpdateStatusBasedOnDates(int id);
        Task UpdateStatusLoad(int id);
        Task UpdateAsync(int id, SaleDTO saleDto);
        Task DeleteAsync(int id);
    }
}
