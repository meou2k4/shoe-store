using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModel
{
    public class HoadonchitietViewModel
    {
        public int Id {  get; set; }
        public int Idhd {  get; set; }
        public int Idspct {  get; set; }
        public int Idsp { get; set; }
        public string Tensp {  get; set; }
        public string? urlHinhanh {  get; set; }
        public decimal Giasp {  get; set; }
        public decimal? Giamgia {  get; set; }
        public int Soluong {  get; set; }
        public int Trangthai { get; set; }
        public string Mau { get; set; }
        public int Size { get; set; }
        public string Chatlieu { get; set; }
    }

    public class TraHangchitietViewModel
    {
        public int Id { get; set; }
        public int trahang { get; set; }
        public int Idspct { get; set; }
        public int Idsp { get; set; }
        public string Tensp { get; set; }
        public string? urlHinhanh { get; set; }
        public decimal Giasp { get; set; }
        public decimal? Giamgia { get; set; }
        public int Soluong { get; set; }
        public int Trangthaihd { get; set; }
        public string Mau { get; set; }
        public int Size { get; set; }
        public string Chatlieu { get; set; }
    }
}
