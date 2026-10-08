using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Giohangchitiet
	{

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

		public int Idgh { get; set; }
		[ForeignKey("Idgh")]
		public virtual Giohang Giohang { get; set; }

		public int Idspct { get; set; }
		[ForeignKey("Idspct")]
		public virtual Sanphamchitiet Sanphamchitiet {  get; set; }

		public int Soluong {  get; set; }
	}
}
