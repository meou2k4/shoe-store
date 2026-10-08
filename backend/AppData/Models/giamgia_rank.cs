using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class giamgia_rank
	{
		[Key, Column(Order = 0)]
		public int IDgiamgia { get; set; }

		[Key, Column(Order = 1)]
		public int Idrank { get; set; }

	
		[ForeignKey("IDgiamgia")]
		public virtual Giamgia Giamgia { get; set; }
		[ForeignKey("Idrank")]
		public virtual Rank Rank { get; set; }
	}
}
