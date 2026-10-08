using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class NhanvienDTO
	{
		public int Id { get; set; }
		public string Hovaten { get; set; }
		public DateTime Ngaysinh { get; set; }
		public string Diachi { get; set; }
		public bool Gioitinh { get; set; } 
		public string Sdt { get; set; }
		public string Email { get; set; }
		public int Trangthai { get; set; }
		public string Password { get; set; }
        public int Role { get; set; }

        public DateTime Ngaytaotaikhoan { get; set; }
        public string? Avatar { get; set; }
    }
    public class NhanvienUpdateDTO
    {
        public int Id { get; set; }
        public string Hovaten { get; set; }
        public DateTime Ngaysinh { get; set; }
        public string Diachi { get; set; }
        public bool Gioitinh { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string? Avatar { get; set; }
    }
    public class ThongTinNhanVienDTO
    {
        public int Id { get; set; }
        public string Hovaten { get; set; }
        public DateTime Ngaysinh { get; set; }
        public string Diachi { get; set; }
        public bool Gioitinh { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
    }
}
