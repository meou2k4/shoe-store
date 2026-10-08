using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.ViewModel
{
    public class SanphamViewModel
    {
        public int Id { get; set; }
        public string Tensp { get; set; }
        public string Mota { get; set; }
        public decimal Giaban { get; set; }
        public decimal? Giasale { get; set; }
        public DateTime NgayThemSanPham { get; set; }
        public int TrangThai {  get; set; }
        public string URlHinhAnh { get; set; }
        public int Soluong { get; set; }
        public string ThuongHieu { get; set; }
        public int idThuongHieu { get; set; }
        public decimal GiaTriGiam { get; set; }
        public float trungBinhDanhGia { get; set; }

        public List<SanphamchitietViewModel> Sanphamchitiets { get; set; }
    }

    public class SanphamchitietViewModel
    {
        public int Id { get; set; }
        public decimal Giathoidiemhientai { get; set; }
        public decimal? GiaSaleSanPhamChiTiet { get; set; }
        public int Soluong { get; set; }
        public int TrangThai { get; set; }
        public string? UrlHinhanh { get; set; }
        public int soLuongBan { get; set; }
        public int IdSize { get; set; }
        public int IdChatLieu { get; set; }
        public int IdMau { get; set; }
        public List<SalechitietViewModel>? Sales { get; set; }

    }

    public class SalechitietViewModel
    {
       public int trangThai { get; set; }
        public int Donvi { get; set; }
        public decimal Giatrigiam { get; set; }
        public int Soluong { get; set; }
        public string TenSale { get; set; } 
        public DateTime Ngaybatdau { get; set; }
        public DateTime Ngayketthuc { get; set; }   


    }

}
