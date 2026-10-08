using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class ChangePasswordDto
	{
		public int UserId { get; set; }   // Id nhân viên
		public string CurrentPassword { get; set; }   // Mật khẩu cũ (plain)
		public string NewPassword { get; set; }   // Mật khẩu mới (plain)
		public string ConfirmPassword { get; set; }   // Xác nhận (plain)
	}
}
