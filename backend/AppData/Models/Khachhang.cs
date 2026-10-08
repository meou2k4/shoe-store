using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Khachhang
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
		[StringLength(50, ErrorMessage = "Tên khách hàng không được vượt quá 50 ký tự.")]
		public string Ten { get; set; }

		[Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
		[Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
		public string Sdt { get; set; }
        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ.")]
        public DateTime Ngaysinh { get; set; }

		[Required(ErrorMessage = "Tích điểm là bắt buộc.")]
		[Range(0, double.MaxValue, ErrorMessage = "Tích điểm phải là số dương.")]
		public decimal Tichdiem { get; set; }

		[EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
		public string? Email { get; set; }

		public string? Password { get; set; }

		public DateTime? Ngaytaotaikhoan { get; set; }

		[Required(ErrorMessage = "Điểm sử dụng là bắt buộc.")]
		[Range(0, int.MaxValue, ErrorMessage = "Điểm sử dụng phải là số dương.")]
		public int Diemsudung { get; set; }

		[Required(ErrorMessage = "Trạng thái là bắt buộc.")]
		public int Trangthai { get; set; }

		public int Idrank { get; set; }
		[ForeignKey("Idrank")]
		public virtual Rank Rank { get; set; }

        public bool Gioitinh { get; set; } // true: Nam, false: Nữ

        public Giohang Giohang { get; set; }
        public virtual ICollection<Danhgia> Danhgias { get; set; }
		public virtual ICollection<Diachi> Diachis { get; set; }
		public virtual ICollection<Hoadon> Hoadons { get; set; }
		public string Avatar { get; set; }
    }
}
