using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Giohang
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id {  get; set; }

		public int Soluong {  get; set; }

		public int Idkh { get; set; }
		public  Khachhang Khachhang { get; set; }

		public virtual ICollection<Giohangchitiet> Giohangchitiets { get; set; }
    }

}
