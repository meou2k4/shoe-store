using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IService_Admin
{
	public interface ISaleService
	{
		Task<bool> UpdateSaleWithDetailsAsync(int saleId, CreateSaleDto updateSaleDto);
		Task<IEnumerable<Sale>> GetAllWithIdAsync();
		Task<Sale> GetByIdAsync(int id);
		Task<SaleDto> AddAsync(SaleDto saleDto);
		Task UpdateStatusToCancelled(int id);
		Task UpdateStatusBasedOnDates(int id);
		Task UpdateStatusLoad(int id);
		Task UpdateAsync(int id, SaleDto saleDto);
		Task<bool> DeleteSaleAsync(int id);
		Task<bool> AddSaleWithDetailsAsync(CreateSaleDto createSaleDto);
		Task<bool> UpdateSanphamchitietPricesAsync(int saleId);
		Task<List<SaleDetailDTO>> GetSaleDetailsAsync(int saleId);
	}
}
