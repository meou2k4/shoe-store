using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class DiachiDTO
	{
		public int Id { get; set; }

		public int Idkh { get; set; }

		public string Tennguoinhan { get; set; }

		public string sdtnguoinhan { get; set; }

		public string Thanhpho {  get; set; }

		public string Quanhuyen {  get; set; }

		public string Phuongxa {  get; set; }

		public string? Diachicuthe {  get; set; }
        public string? trangthai { get; set; }
    }
}
