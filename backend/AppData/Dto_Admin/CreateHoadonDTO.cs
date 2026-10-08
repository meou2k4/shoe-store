using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Models;
using AppData.Dto;

namespace AppData.Dto_Admin
{
	public class CreateHoadonDTO
	{
		public int? Idnv { get; set; }

		public decimal Tongtiencantra { get; set; }
		public int Idpttt { get; set; }
		public decimal Tongtiensanpham { get; set; }

		
		public string? Ghichu { get; set; }
		public List<HoadonchitietDto> SanPhamChiTiet { get; set; }
	}
}
