using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;

namespace AppData.IService_Admin
{
	public interface IThongkeService
	{
		Task<List<TopSellingProductDto>> GetTopSellingProductsByTimeAsync(DateTime startDate, DateTime endDate);
	}
}
