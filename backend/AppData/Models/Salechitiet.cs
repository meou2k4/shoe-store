using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Salechitiet
	{	
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

		public int Idspct { get; set; }
		[ForeignKey("Idspct")]
		public virtual Sanphamchitiet spchitiet {  get; set; }

		public int Idsale { get; set; }
		[ForeignKey("Idsale")]
		public virtual Sale Sale {  get; set; }

		[Range(0, 1, ErrorMessage = "Đơn vị phải là 0 (VND) hoặc 1 (%)")]
		public int Donvi { get; set; }  // Đổi kiểu về int

		[Required(ErrorMessage = "Số lượng không được để trống")]
		//[Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1")]
		public int Soluong { get; set; }  // Thêm thuộc tính Required

		[Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn hoặc bằng 0")]
		public decimal Giatrigiam { get; set; }
	}
}
