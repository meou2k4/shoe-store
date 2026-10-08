using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class DanhgiaDTO
	{
		public int Id {  get; set; }

		public int Idkh { get; set; }

		public string? Noidungdanhgia {  get; set; }

		public DateTime Ngaydanhgia { get; set; }

		public int	Idhdct { get; set; }

        public int Sosao { get; set; }
        public List<HinhanhDTO>? Hinhanhs { get; set; } = new List<HinhanhDTO>();
    }
}
