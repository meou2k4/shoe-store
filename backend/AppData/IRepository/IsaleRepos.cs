using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IRepository
{
	public interface IsaleRepos
	{
		Task<List<UpdateGiaSanphamchitietDTO>> GetSanphamchitietForUpdateAsync(int saleId);
		Task<IEnumerable<Sale>> GetAllAsync();
		Task<Sale> GetByIdAsync(int id);
		Task AddAsync(Sale sale);
		Task UpdateAsync(Sale sale);
		Task DeleteSaleAsync(Sale sale);
		Task SaveChangesAsync();
		Task DeleteSaleChitietsBySaleIdAsync(int saleId);
		Task<List<SaleDetailDTO>> GetSaleDetailsBySaleIdAsync(int saleId);
		Task AddSaleAsync(Sale sale);
	
	}
}
