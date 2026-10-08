using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.ComponentModel;

namespace AppData.DTO
{
    public class HoadonDTO
    {
        public int Id { get; set; }
        public int? Idnv { get; set; }
        public int? Idkh { get; set; }
        public int Trangthaithanhtoan { get; set; }
        public int Trangthaidonhang { get; set; }
        public DateTime Thoigiandathang { get; set; }
        public string? Diachiship { get; set; }
        public DateTime? Ngaygiaothucte { get; set; }
        public decimal Tongtiencantra { get; set; }
        public decimal Tongtiensanpham { get; set; }
        public string? Sdt { get; set; }
        public decimal? Tonggiamgia { get; set; }
        public int? Idgg { get; set; }
        public int Trangthai { get; set; }
        public decimal Phivanchuyen { get; set; }
        public int Idpttt { get; set; }
        public string? Ghichu { get; set; }
    }
    public class OrderNotificationDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; }
        public bool IsRead { get; set; }
        public string Status { get; set; }
    }
}
