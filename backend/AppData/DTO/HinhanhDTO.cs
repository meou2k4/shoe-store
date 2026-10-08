using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class HinhanhDTO
	{
		public int Id {  get; set; }

		public string Urlhinhanh { get; set; }
        public int? Idtrahang { get; set; }
        public int? Iddanhgia { get; set; }
    }
}
