using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AppData.Models;

namespace AppData
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Danhgia> danhgias { get; set; }
        public DbSet<Diachi> diachis { get; set; }
        public DbSet<giamgia_rank> giamgia_Ranks { get; set; }
        public DbSet<Giamgia> giamgias { get; set; }
        public DbSet<Giohang> giohangs { get; set; }
        public DbSet<Giohangchitiet> giohangchitiets { get; set; }
        public DbSet<Hinhanh> hinhanhs { get; set; }
        public DbSet<Hoadon> hoadons { get; set; }
        public DbSet<Hoadonchitiet> hoadonchitiets { get; set; }
        public DbSet<Khachhang> khachhangs { get; set; }
        public DbSet<Nhanvien> nhanviens { get; set; }
        public DbSet<Phuongthucthanhtoan> phuongthucthanhtoans { get; set; }
        public DbSet<Rank> ranks { get; set; }
        public DbSet<Sale> sales { get; set; }
        public DbSet<Salechitiet> salechitiets { get; set; }
        public DbSet<Sanpham> sanphams { get; set; }
        public DbSet<Sanphamchitiet> Sanphamchitiets { get; set; }
        public DbSet<Thuonghieu> thuonghieus { get; set; }
        public DbSet<Trahang> trahangs { get; set; }
        public DbSet<Trahangchitiet> trahangchitiets { get; set; }
        public DbSet<Color> colors { get; set; }
        public DbSet<Size> sizes { get; set; }
        public DbSet<ChatLieu> chatLieus { get; set; }
        //asdfgfds
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<giamgia_rank>()
            .HasKey(l => new { l.IDgiamgia, l.Idrank });
            modelBuilder.Entity<Giohang>()
                .HasOne(g => g.Khachhang)           // Giỏ hàng có một Khách hàng
                .WithOne(k => k.Giohang)            // Khách hàng có một Giỏ hàng
                .HasForeignKey<Giohang>(g => g.Idkh);
            modelBuilder.Entity<Danhgia>()
                .HasOne(g => g.Hoadonchitiet)           // Giỏ hàng có một Khách hàng
                .WithOne(k => k.danhgia)            // Khách hàng có một Giỏ hàng
                .HasForeignKey<Danhgia>(g => g.Idhdct);
            modelBuilder.Entity<Trahangchitiet>()
                .HasOne(g => g.Hoadonchitiet)           // Giỏ hàng có một Khách hàng
                .WithOne(k => k.Trahangchitiet)            // Khách hàng có một Giỏ hàng
                .HasForeignKey<Trahangchitiet>(g => g.Idhdct);
            //modelBuilder.Entity<Salechitiet>()
            /*.HasOne(s => s.Sanpham)
            .WithMany(p => p.Salechitiets)
            .HasForeignKey(s => s.Idsp)
            .OnDelete(DeleteBehavior.Restrict);
             modelBuilder.Entity<Danhgia>()*/
            /*  .HasOne(s => s.Khachhang)
              .WithMany(p => p.Danhgias)
              .HasForeignKey(s => s.Idkh)*/
            /*.OnDelete(DeleteBehavior.Restrict);*/
            modelBuilder.Entity<Danhgia>()
           .HasOne(s => s.Khachhang)
           .WithMany(p => p.Danhgias)
           .HasForeignKey(s => s.Idkh)
           .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Trahangchitiet>()
           .HasOne(s => s.Trahang)
           .WithMany(p => p.Trahangchitiets)
           .HasForeignKey(s => s.Idth)
           .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
