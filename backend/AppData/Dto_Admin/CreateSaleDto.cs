using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class CreateSaleDto
	{
		[Required(ErrorMessage = "Tên không được để trống")]
		[StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
		public string Ten { get; set; }

		[StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
		public string? Mota { get; set; }

		[Range(0, 3, ErrorMessage = "Trạng thái phải là 0, 1, 2 hoặc 3")]
		public int Trangthai { get; set; }

		[Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
		public DateTime Ngaybatdau { get; set; }

		[Required(ErrorMessage = "Ngày kết thúc không được để trống")]
		public DateTime Ngayketthuc { get; set; }

		public List<CreateSaleDetailDto> SaleDetails { get; set; }
	}
}
