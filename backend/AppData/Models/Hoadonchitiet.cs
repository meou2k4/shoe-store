using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Hoadonchitiet

	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

		public int Idhd { get; set; }
		[ForeignKey("Idhd")]
		public virtual Hoadon Hoadon {  get; set; }

		public int Idspct { get; set; }
		[ForeignKey("Idspct")]
		public virtual Sanphamchitiet Idspchitiet {  get; set; }

		public int Soluong {  get; set; }

		public decimal Giasp {  get; set; }
	
		public decimal? Giamgia {  get; set; }

		public virtual Trahangchitiet Trahangchitiet { get; set; }
		
		public virtual Danhgia danhgia {  get; set; }
	}
}
