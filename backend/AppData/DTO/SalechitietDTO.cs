using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class SalechitietDTO
	{	
		public int Id {  get; set; }
		public int Idspct { get; set; }
		public int Idsale { get; set; }
		public int Donvi { get; set; } 
		public int Soluong { get; set; } 
		public decimal Giatrigiam { get; set; }
	}
}
