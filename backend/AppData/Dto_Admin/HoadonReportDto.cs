using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class HoadonReportDto
	{
		public int Year { get; set; }
		public int Week { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalAmount { get; set; }
	}
}
