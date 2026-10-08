using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class LatestInvoiceDto
	{
		public string MaHoaDon { get; set; }   
		public string TenKhachHang { get; set; }   
		public decimal TongTien { get; set; }
		public int TrangThai { get; set; }  
	}
}
