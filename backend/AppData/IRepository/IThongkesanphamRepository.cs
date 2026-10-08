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
	public interface IThongkesanphamRepository
	{
		Task<List<TopSellingProductDto>> GetTopSellingProductsByTimeAsync(DateTime startDate, DateTime endDate);
		Task<List<CustomerStatisticsDTO>> GetTop5CustomersAsync(DateTime month);
		Task<List<Khachhang>> GetActiveCustomersAsync();
	}
}
