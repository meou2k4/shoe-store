using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class KhachhangDTO
	{
		public int Id { get; set; }
		public string Ten { get; set; }
		public string Sdt { get; set; }
		public DateTime Ngaysinh { get; set; }
		public decimal Tichdiem { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public DateTime? Ngaytaotaikhoan { get; set; }
		public int Diemsudung { get; set; }
		public int Trangthai { get; set; }
		public int Idrank { get; set; }
        public bool Gioitinh { get; set; }
		public string Avatar { get; set; }
    }
}
