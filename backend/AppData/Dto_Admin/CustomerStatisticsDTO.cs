using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class CustomerStatisticsDTO
	{
		public string CustomerName { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalSpent { get; set; }
		public int SuccessfulOrders { get; set; }
		public int CanceledOrders { get; set; }
		public int ReturnedOrders { get; set; }
	}
}
