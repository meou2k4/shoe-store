using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class Giamgia_RankDTO
	{
		public string? Mota { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn đơn vị")]
		[Range(0, 1, ErrorMessage = "Đơn vị phải là 0 hoặc 1")]
		public int Donvi { get; set; }//0 là VND, 1 là %
		public int Soluong { get; set; }
		public int Giatri { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
		public DateTime Ngaybatdau { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
		public DateTime Ngayketthuc { get; set; }
		[Range(0, 4, ErrorMessage = "Phải lựa chọn trạng thái")]//0: phát hành, 1: chuẩn bị phát hành, 2: dừng phát hành
		public int Trangthai { get; set; }
		public List<string> RankNames { get; set; }
	}
}
