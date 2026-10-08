using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto_Admin
{
	public class ResetPasswordDto
	{
		public string Email { get; set; } = null!;
		public string NewPassword { get; set; } = null!;
	}

}
