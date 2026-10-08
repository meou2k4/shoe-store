using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Models
{
	public class Rank
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id {  get; set; }

        [Required(ErrorMessage = "Tên rank là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên rank không được quá 100 ký tự.")]
        public string Tenrank { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "MinMoney phải lớn hơn hoặc bằng 0.")]
        public decimal MinMoney { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "MaxMoney phải lớn hơn hoặc bằng 0.")]
        public decimal MaxMoney { get; set; }


        // Kiểm tra Trangthai phải là 0 hoặc 1
        [Range(0, 2, ErrorMessage = "Trạng thái phải là 0 hoặc 2.")]//2 là xóa mềm
        public int Trangthai { get; set; }

        public virtual ICollection<Khachhang> Khachhangs { get; set; }
		public virtual ICollection<giamgia_rank> Giamgia_Ranks { get; set; }
	}
}
