using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Trahangchitiet
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

        
        public int Idth {  get; set; }
		[ForeignKey("Idth")]
		public virtual Trahang Trahang { get; set; }

		public int Soluong {  get; set; }

		public int Tinhtrang {  get; set; }
		public int Idhdct { get; set; }
		public  Hoadonchitiet Hoadonchitiet { get; set; }
	}
}
