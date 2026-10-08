using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto
{
	public class DoimkKhachhang
	{

		[Required(ErrorMessage = "Vui lòng nhập email")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
		[StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 20 ký tự")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
		[StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 20 ký tự")]
		public string NewPassword { get; set; }
	}
}
