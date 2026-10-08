using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class PhuongthucthanhtoanDTO
	{
		public int Id { get; set; }
		public string Tenpttt { get; set; }
		public int Trangthai { get; set; }
	}
}
