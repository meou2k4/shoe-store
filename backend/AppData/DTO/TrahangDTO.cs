using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AppData.DTO
{
    public class TrahangDTO
	{
        public int Id {  get; set; }
        public int Idhd { get; set; }
        public string? Diachiship { get; set; }
        public string Tenkhachhang {  get; set; }
        public int Trangthaihoantien { get; set; }
        public int? Idnv { get; set; }
		public int Idkh { get; set; }
		public decimal Sotienhoan {  get; set; }
		public string? Lydotrahang {  get; set; }
		public int Trangthai {  get; set; }
		public string Phuongthuchoantien {  get; set; }
        public DateTime Ngaytaodon { get; set; }
        public DateTime? Ngaytrahangthucte {  get; set; }
		public string? Chuthich {  get; set; }
        public string? Hinhthucxuly { get; set; }
		public string? Tennganhang { get; set; }
        public string? Sotaikhoan { get; set; }
        public string? Tentaikhoan { get; set; }
    }

    public class TrahangtaoDTO
    {
        public int Id { get; set; }
        public string? Diachiship { get; set; }
        public string Tenkhachhang { get; set; }
        public int Trangthaihoantien { get; set; }
        public int? Idnv { get; set; }
        public int Idkh { get; set; }
        public decimal Sotienhoan { get; set; }
        public string? Lydotrahang { get; set; }
        public int Trangthai { get; set; }
        public string Phuongthuchoantien { get; set; }
        public DateTime Ngaytaodon { get; set; }
        public DateTime? Ngaytrahangthucte { get; set; }
        public string? Chuthich { get; set; }
        public string? Hinhthucxuly { get; set; }
        public string? Tennganhang { get; set; }
        public string? Sotaikhoan { get; set; }
        public string? Tentaikhoan { get; set; }
    }
}
