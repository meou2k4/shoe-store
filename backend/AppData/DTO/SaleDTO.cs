using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class SaleDTO
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string? Mota { get; set; }
		public int Trangthai { get; set; }  
		public DateTime Ngaybatdau { get; set; }
		public DateTime Ngayketthuc { get; set; }
	}
}
