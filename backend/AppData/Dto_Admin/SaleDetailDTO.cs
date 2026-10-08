using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class SaleDetailDTO
	{
		public string ProductName { get; set; }
		public string Unit { get; set; }
		public decimal Price { get; set; }
		public decimal Discount { get; set; }
		public int Quantity { get; set; }
		public int? idspct { get; set; }
		public decimal SalePrice { get; set; }

	}
}
