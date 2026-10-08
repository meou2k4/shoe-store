using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class HoadonoffKhachhangthanthietDto
	{
		public int? Idnv { get; set; }
		public int? Idkh { get; set; }
		public int Idpttt { get; set; }
		public decimal Tongtiencantra { get; set; }

		public decimal Tongtiensanpham { get; set; }
		public string? Sdt { get; set; }
		public decimal? Tonggiamgia { get; set; }

		public int? Idgg { get; set; }
		public string? Ghichu { get; set; }
		public List<HoadonchitietDto> SanPhamChiTiet { get; set; }
	}
}
