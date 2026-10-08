using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
    public class SanphamDTO
    {
		public int Id { get; set; }
		public string TenSanpham { get; set; }
		public string? Mota { get; set; }
		public int Trangthai { get; set; }
        public int Soluong { get; set; }
        public int Chieucao { get; set; }
        public decimal GiaBan { get; set; }
        public DateTime NgayThemMoi { get; set; }
        public int Chieudai { get; set; }
        public int Chieurong { get; set; }
        public int Trongluong { get; set; }
        public int Idth { get; set; }
    }
}
