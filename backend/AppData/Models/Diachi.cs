using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Diachi
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int Idkh { get; set; }
		[ForeignKey("Idkh")]
		public virtual Khachhang Khachhang {  get; set; }

		public string Tennguoinhan { get; set; }

		public string sdtnguoinhan { get; set; }

		public string Thanhpho {  get; set; }

		public string Quanhuyen {  get; set; }

		public string Phuongxa {  get; set; }

		public string? Diachicuthe {  get; set; }

        public string? trangthai { get; set; }
    }
}
