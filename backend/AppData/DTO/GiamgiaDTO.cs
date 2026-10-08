using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.DTO
{
	public class GiamgiaDTO
	{
		public int Id { get; set; }
		public string? Mota { get; set; }
		public int Donvi { get; set; }
		public int Soluong { get; set; }
		public int Giatri { get; set; }
		public DateTime Ngaybatdau { get; set; }
		public DateTime Ngayketthuc { get; set; }
		public int Trangthai { get; set; }

        public List<string> RankNames { get; set; }
    }
    public class GiamgiaDTOupdate
    {
        public int Id { get; set; }
        public string? Mota { get; set; }
        public int Donvi { get; set; }
        public int Soluong { get; set; }
        public int Giatri { get; set; }
        public DateTime Ngaybatdau { get; set; }
        public DateTime Ngayketthuc { get; set; }
        public int Trangthai { get; set; }
    }
    public class GiamgiaDTOadd
    {
        public int Id { get; set; }
        public string? Mota { get; set; }
        public int Donvi { get; set; }
        public int Soluong { get; set; }
        public int Giatri { get; set; }
        public DateTime Ngaybatdau { get; set; }
        public DateTime Ngayketthuc { get; set; }
    }
}
