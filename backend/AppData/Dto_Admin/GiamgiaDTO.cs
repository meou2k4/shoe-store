using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class GiamgiaDTO
	{
		public string? Mota { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn đơn vị")]
		[Range(0, 1, ErrorMessage = "Đơn vị phải là VND hoặc %")]
		public int Donvi { get; set; }//0 là VND, 1 là %
		public int Soluong { get; set; }
		public int Giatri { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
		public DateTime Ngaybatdau { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
		public DateTime Ngayketthuc { get; set; }
		[Range(0, 4, ErrorMessage = "Phải lựa chọn trạng thái")]
		public int Trangthai { get; set; }
	}
}
