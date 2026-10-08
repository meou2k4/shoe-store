using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Nhanvien
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập họ tên")]
		[StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 ký tự")]
		public string Hovaten { get; set; }

		[Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
		[DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ")]
		public DateTime Ngaysinh { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
		[StringLength(100, ErrorMessage = "Địa chỉ không được vượt quá 100 ký tự")]
		public string Diachi { get; set; }

		[Required(ErrorMessage = "Vui lòng chọn giới tính")]
		public bool Gioitinh { get; set; } // true: Nam, false: Nữ

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[RegularExpression(@"^(\d{10})$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
		public string Sdt { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập email")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ")]
		[StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Trạng thái không được để trống")]
		[Range(0, 2, ErrorMessage = "Trạng thái chỉ có thể là 0 hoặc 1")]
		public int Trangthai { get; set; }// 0: Hoạt động, 1: Dừng hoạt động


		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string Password { get; set; }

        [Required(ErrorMessage = "Vai trò không được để trống")]
		[Range(0, 2, ErrorMessage = "Vai trò là nhân viên hoặc quản lý")]
		public int Role { get; set; }// 0: Quản lý, 1: Nhân viên

        public DateTime Ngaytaotaikhoan { get; set; }

        public string? Avatar { get; set; }

        public virtual ICollection<Hoadon> Hoadons { get; set; }
		public virtual ICollection<Trahang> Trahangs { get; set; }
	}
}
