using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.IRepository;
using AppData.IService_Admin;

namespace AppData.Service_Admin
{
	public class ThongkeService : IThongkeService
	{
		private readonly IThongkesanphamRepository _repository;
        public ThongkeService(IThongkesanphamRepository repository)
        {
			_repository = repository;

		}
		public async Task<List<TopSellingProductDto>> GetTopSellingProductsByTimeAsync(DateTime startDate, DateTime endDate)
		{
			return await _repository.GetTopSellingProductsByTimeAsync(startDate, endDate);  // Gọi phương thức bất đồng bộ từ Repository
		}

	}
}
