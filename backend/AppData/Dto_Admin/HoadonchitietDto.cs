using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class HoadonchitietDto
	{
		public int Idspct { get; set; } // ID sản phẩm chi tiết
		public int Soluong { get; set; } // Số lượng
		public decimal Giasp { get; set; } // Giá sản phẩm
		public decimal? Giamgia { get; set; } 
	}
}
