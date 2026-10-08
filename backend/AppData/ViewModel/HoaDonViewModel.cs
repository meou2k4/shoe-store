using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModel
{
    public class HoaDonViewModel
    {
        public int Id { get; set; } 
        public decimal Tongtiencantra {  get; set; }
        public decimal Tongtiensanpham {  get; set; }
        public decimal Giamgia { get; set; }
        public int Trangthaidonhang { get; set; }
        public DateTime? Thoigiandathang { get; set; }
        public int Trangthaithanhtoan {  get; set; }
        public string Diachiship {  get; set; }
        public int Tongsoluong {  get; set; }
        public DateTime? Ngaygiaothucte { get; set; }
        public int Trangthai { get; set; }
    }
}
