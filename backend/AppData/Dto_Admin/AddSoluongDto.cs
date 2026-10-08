using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class AddSoluongDto
	{
		[Required(ErrorMessage = "Số lượng cần thêm không được để trống")]
		[Range(0, int.MaxValue, ErrorMessage = "Số lượng cần thêm phải lớn hơn 0")]
		public int SoluongThem { get; set; }
	}
}
