using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class SanphamchitietDTO
	{
		public int Id {  get; set; }
		public int Trangthai { get; set; }
		public decimal Giathoidiemhientai { get; set; }
		public int Soluong {  get; set; }
        public string? UrlHinhanh { get; set; }
		public int Idsp { get; set; }
		public int IdSize { get; set; }
		public int IdChatLieu { get; set; }
		public int IdMau { get; set; }
		
	}
}
