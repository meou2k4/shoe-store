using AppData.IRepository;
using AppData.Models;
using AppData.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class TraHangChiTietRepo : ITraHangChiTietRepo
    {
        private readonly AppDbContext _context;
        public TraHangChiTietRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(Trahangchitiet ct)
        {
            await _context.trahangchitiets.AddAsync(ct);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var a = await _context.trahangchitiets.FirstOrDefaultAsync(x => x.Id == id);
            if (a != null)
            {
                _context.trahangchitiets.Remove(a);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tồn tại!");
            }
        }

        public async Task<List<Trahangchitiet>> GetAll()
        {
            return await _context.trahangchitiets.ToListAsync();
        }

        public async Task<Trahangchitiet> GetById(int id)
        {
            return await _context.trahangchitiets.FindAsync(id);
        }

        public async Task<List<HoadonchitietViewModel>> ListSanPhamByIdhd(int id)
        {
            var data = await _context.hoadonchitiets
                .Where(hdct => hdct.Idhd == id)
                .Join(_context.Sanphamchitiets, hdct => hdct.Idspct, spct => spct.Id, (hdct, spct) => new { hdct, spct })
                .Join(_context.sanphams, x => x.spct.Idsp, sp => sp.Id, (x, sp) => new { x.hdct, x.spct, sp })
                .Join(_context.hoadons, x => x.hdct.Idhd, hd => hd.Id, (x, hd) => new { x.hdct, x.spct, x.sp, hd })
                .Join(_context.colors, x => x.spct.IdMau, mau => mau.Id, (x, mau) => new { x.hdct, x.spct, x.sp, x.hd, mau })
                .Join(_context.sizes, x => x.spct.IdSize, size => size.Id, (x, size) => new { x.hdct, x.spct, x.sp, x.hd, x.mau, size })
                .Join(_context.thuonghieus,x=>x.sp.Idth, th => th.Id, (x, th) => new { x.hdct, x.spct, x.sp, x.hd, x.mau, x.size, th })
                .Join(_context.chatLieus, x => x.spct.IdChatLieu, chatlieu => chatlieu.Id, (x, chatlieu) => new HoadonchitietViewModel
                {
                    Id = x.hdct.Id,
                    Idhd = id,
                    Idsp = x.sp.Id,
                    Idspct = x.spct.Id,
                    Tensp = $"[{x.th.Tenthuonghieu}] -  {x.sp.TenSanpham}",
                    urlHinhanh = x.spct.UrlHinhanh,
                    Giasp = x.hdct.Giasp,
                    Giamgia = x.hdct.Giamgia,
                    Soluong = x.hdct.Soluong,
                    Trangthai = x.hd.Trangthai,
                    Mau = x.mau.Tenmau,
                    Size = x.size.Sosize,
                    Chatlieu = chatlieu.Tenchatlieu
                })
                .AsNoTracking() // Giúp cải thiện hiệu suất
                .ToListAsync();

            return data ?? new List<HoadonchitietViewModel>(); // Tránh trả về null
        }


        public async Task Update(Trahangchitiet ct)
        {
            _context.trahangchitiets.Update(ct);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SanPhamTraHang>> SanphamByThct()
        {
            try
            {
                List<SanPhamTraHang> a = await (from thct in _context.trahangchitiets
                                                join hdct in _context.hoadonchitiets on thct.Idhdct equals hdct.Id
                                                join spct in _context.Sanphamchitiets on hdct.Idspct equals spct.Id
                                                join sp in _context.sanphams on spct.Idsp equals sp.Id
                                                join thuonghieu in _context.thuonghieus on sp.Idth equals thuonghieu.Id
                                                join size in _context.sizes on spct.IdSize equals size.Id
                                                join chatlieu in _context.chatLieus on spct.IdChatLieu equals chatlieu.Id
                                                join mau in _context.colors on spct.IdMau equals mau.Id
                                                select new SanPhamTraHang
                                                {
                                                    Id = thct.Id,
                                                    Idth = thct.Idth,
                                                    Soluong = thct.Soluong,
                                                    Tinhtrang = thct.Tinhtrang,
                                                    Idsp = sp.Id,
                                                    UrlAnh = spct.UrlHinhanh,
                                                    Tensp = sp.TenSanpham,
                                                    Tenthuonghieu = thuonghieu.Tenthuonghieu,
                                                    Giasp = hdct.Giasp - (hdct.Giamgia ?? 0),
                                                    Tenmau = mau.Tenmau,
                                                    Tensize = size.Sosize,
                                                    Tenchatlieu = chatlieu.Tenchatlieu
                                                }).ToListAsync();
                return a;
            }
            catch(Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
            
        }
        public async Task<List<TraHangchitietViewModel>> ListSanPhamByIdth(int id)
        {
            // Bước 1: Lấy danh sách trahangchitiet theo Idth
            var traHangChiTiets = await _context.trahangchitiets
                .Where(thct => thct.Idth == id)
                .ToListAsync();

            if (!traHangChiTiets.Any())
                return new List<TraHangchitietViewModel>();

            // Chuyển sang Dictionary để lookup nhanh ở bước 3
            var traHangDict = traHangChiTiets.ToDictionary(t => t.Idhdct);

            // Bước 2 + 3: Lấy hoadonchitiet theo danh sách Idhdct, join các bảng liên quan
            var raw = await _context.hoadonchitiets
                .Where(hdct => traHangDict.Keys.Contains(hdct.Id))
                .Join(_context.Sanphamchitiets,
                      hdct => hdct.Idspct, spct => spct.Id,
                      (hdct, spct) => new { hdct, spct })
                .Join(_context.sanphams,
                      x => x.spct.Idsp, sp => sp.Id,
                      (x, sp) => new { x.hdct, x.spct, sp })
                .Join(_context.colors,
                      x => x.spct.IdMau, mau => mau.Id,
                      (x, mau) => new { x.hdct, x.spct, x.sp, mau })
                .Join(_context.sizes,
                      x => x.spct.IdSize, size => size.Id,
                      (x, size) => new { x.hdct, x.spct, x.sp, x.mau, size })
                .Join(_context.thuonghieus,
                      x => x.sp.Idth, th => th.Id,
                      (x, th) => new { x.hdct, x.spct, x.sp, x.mau, x.size, th })
                .Join(_context.chatLieus,
                      x => x.spct.IdChatLieu, chatlieu => chatlieu.Id,
                      (x, chatlieu) => new { x.hdct, x.spct, x.sp, x.mau, x.size, x.th, chatlieu })
                .AsNoTracking()
                .ToListAsync();

            // Map về ViewModel, lấy Id và Soluong từ trahangchitiets
            var result = raw
                .Select(x =>
                {
                    var thct = traHangDict[x.hdct.Id];
                    return new TraHangchitietViewModel
                    {
                        Id = thct.Id,
                        Idsp = x.sp.Id,
                        Idspct = x.spct.Id,
                        Tensp = $"[{x.th.Tenthuonghieu}] - {x.sp.TenSanpham}",
                        urlHinhanh = x.spct.UrlHinhanh,
                        Giasp = x.hdct.Giasp,
                        Giamgia = x.hdct.Giamgia,
                        Soluong = thct.Soluong,
                        Mau = x.mau.Tenmau,
                        Size = x.size.Sosize,
                        Chatlieu = x.chatlieu.Tenchatlieu
                    };
                })
                .ToList();

            return result;
        }
    }
}
