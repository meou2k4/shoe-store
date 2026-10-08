using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Thuonghieu
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

		[Required(ErrorMessage = "Tên thương hiệu không được để trống")]
		[MaxLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự")]
		public string Tenthuonghieu { get; set; }

        [Range(0, 2, ErrorMessage = "Tình trạng không hợp lệ")]// 0: Hoạt động, 1: Dừng hoạt động 2 là đã xóa
        public int Tinhtrang { get; set; } 

		public virtual ICollection<Sanpham> Sanphams { get; set; }
	}
}
