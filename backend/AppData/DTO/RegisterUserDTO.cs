using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Dto
{
	public class RegisterUserDTO
	{
		public string Ten { get; set; }
		public string Sdt { get; set; }
		public DateTime Ngaysinh { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public bool gioitinh { get; set; }

	}
}
