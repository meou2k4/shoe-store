using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class HoadonchitietDTO

	{
		public int Id {  get; set; }

		public int Idhd { get; set; }

		public int Idspct { get; set; }

		public int Soluong {  get; set; }

		public decimal Giasp {  get; set; }
	
		public decimal? Giamgia {  get; set; }
	}
}
