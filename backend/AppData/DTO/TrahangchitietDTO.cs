using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class TrahangchitietDTO
	{
		public int Id {  get; set; }
		public int Idth {  get; set; }
		public int Soluong {  get; set; }
		public int Tinhtrang {  get; set; }
		public int Idhdct { get; set; }
	}
}
