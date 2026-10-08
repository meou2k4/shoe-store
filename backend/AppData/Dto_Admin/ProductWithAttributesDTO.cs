using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class ProductWithAttributesDTO
	{
		public int Idsp { get; set; }
		public string Tensp { get; set; }
		public decimal Giaban { get; set; }

		public int Idspct { get; set; }

		public int Soluong { get; set; }
		public decimal Giathoidiemhientai { get; set; }

		public string SPCTAttributes { get; set; }
	}
}
