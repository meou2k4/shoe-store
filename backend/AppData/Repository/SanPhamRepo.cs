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
    public class SanPhamRepo : ISanPhamRepo
    {

        private readonly AppDbContext _context;

        public SanPhamRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Sanpham>> GetAllAsync() => await _context.sanphams.ToListAsync();

        public async Task<Sanpham> GetByIdAsync(int id) => await _context.sanphams.FindAsync(id);

        public async Task AddAsync(Sanpham sanpham)
        {
            if (sanpham.Soluong > 0)
                sanpham.Trangthai = 0; // Đang bán
            else
                sanpham.Trangthai = 1; // Hết hàng

            _context.sanphams.Add(sanpham);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sanpham sanpham)
        {
            _context.sanphams.Update(sanpham);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var sanpham = await _context.sanphams.FindAsync(id);
                if (sanpham == null)
                    throw new Exception("Không tìm thấy sản phẩm.");

                var variants = await _context.Sanphamchitiets.Where(v => v.Idsp == id).ToListAsync();
                bool hasReferencedVariant = false;

                foreach (var variant in variants)
                {
                    bool isReferenced = await _context.hoadonchitiets.AnyAsync(x => x.Idspct == variant.Id)
                                     || await _context.giohangchitiets.AnyAsync(x => x.Idspct == variant.Id)
                                     || await _context.salechitiets.AnyAsync(x => x.Idspct == variant.Id);

                    if (isReferenced)
                    {
                        // Nếu được tham chiếu → không xóa mà chuyển trạng thái
                        variant.Soluong = 0;
                        variant.Trangthai = 3;
                        _context.Sanphamchitiets.Update(variant);
                        hasReferencedVariant = true;
                    }
                    else
                    {
                        // Không được tham chiếu → có thể xóa
                        _context.Sanphamchitiets.Remove(variant);
                    }
                }

                if (hasReferencedVariant)
                {
                    sanpham.Trangthai = 3;
                    sanpham.Soluong = 0;
                    _context.sanphams.Update(sanpham);
                }
                else
                {
                    _context.sanphams.Remove(sanpham);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm: " + ex.InnerException?.Message ?? ex.Message);
            }
        }



        public async Task<IEnumerable<Sanpham>> SearchByNameAsync(string name) =>
        await _context.sanphams
        .Where(sp => sp.TenSanpham.ToLower().Contains(name.ToLower()) && sp.Soluong > 0)
        .ToListAsync();


        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamViewModels()
        {
            var sanphams = await _context.sanphams
                .AsNoTracking()
                .Where(sp => sp.Trangthai == 0 || sp.Trangthai == 1)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Salechitiets)
                        .ThenInclude(sale => sale.Sale)
                .Include(sp => sp.Thuonghieu)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Color)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.ChatLieu)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Size)
                .ToListAsync(); 

            if (sanphams == null || !sanphams.Any())
            {
                return new List<SanphamViewModel>(); 
            }

            return sanphams.Select(sanpham => new SanphamViewModel
            {
                Id = sanpham.Id,
                Tensp = sanpham.TenSanpham,
                Mota = sanpham.Mota,
                Giaban = sanpham.GiaBan,
                ThuongHieu = sanpham.Thuonghieu?.Tenthuonghieu ?? "N/A",
                Soluong = sanpham.Soluong,
                idThuongHieu = sanpham.Idth,
                NgayThemSanPham = sanpham.NgayThemMoi,
                TrangThai = sanpham.Trangthai,

                Sanphamchitiets = sanpham.Sanphamchitiets?
               
                    .Where(spct => spct.Trangthai == 0 || spct.Trangthai == 1)
                    .Select(spct => new SanphamchitietViewModel
                    {
                        Id = spct.Id,
                        Giathoidiemhientai = spct.Giathoidiemhientai,
                        TrangThai = spct.Trangthai,
                        Soluong = spct.Soluong,
                        IdChatLieu = spct.IdChatLieu,
                        IdMau = spct.IdMau,
                        IdSize = spct.IdSize,
                        UrlHinhanh = spct.UrlHinhanh,

                        GiaSaleSanPhamChiTiet = spct.Salechitiets?
                            .Where(salect => salect.Sale != null && salect.Sale.Trangthai == 0 && salect.Soluong > 0)
                            .Select(salect =>
                                salect.Donvi == 0
                                    ? spct.Giathoidiemhientai - (decimal)salect.Giatrigiam
                                    : spct.Giathoidiemhientai * (1 - (decimal)salect.Giatrigiam / 100m)
                            ).DefaultIfEmpty(spct.Giathoidiemhientai).Min(),

                        Sales = spct.Salechitiets?
                            .Where(salect => salect.Sale != null && salect.Sale.Trangthai == 0)
                            .Select(salect => new SalechitietViewModel
                            {
                                Donvi = salect.Donvi,
                                Giatrigiam = salect.Giatrigiam,
                                Soluong = salect.Soluong,
                                Ngaybatdau = salect.Sale.Ngaybatdau,
                                Ngayketthuc = salect.Sale.Ngayketthuc,
                                trangThai = salect.Sale.Trangthai
                            }).ToList() ?? new List<SalechitietViewModel>()
                    }).ToList() ?? new List<SanphamchitietViewModel>()
            }).ToList();
        }




        public async Task<SanphamViewModel> GetSanphamViewModelByIdSP(int idsp)
        {
            var sanpham = await _context.sanphams
                .AsNoTracking()
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Salechitiets)
                        .ThenInclude(sale => sale.Sale)
                .Include(sp => sp.Thuonghieu)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Color)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.ChatLieu)
                .Include(sp => sp.Sanphamchitiets)
                    .ThenInclude(spct => spct.Size)
                .Where(sp => sp.Id == idsp)
                .FirstOrDefaultAsync();

            if (sanpham == null)
            {
                return null;
            }

            var sanphamViewModel = new SanphamViewModel
            {
                Id = sanpham.Id,
                Tensp = sanpham.TenSanpham,
                Mota = sanpham.Mota,
                Giaban = sanpham.GiaBan,
                ThuongHieu = sanpham.Thuonghieu?.Tenthuonghieu ?? "N/A",
                Soluong = sanpham.Soluong,
                idThuongHieu = sanpham.Idth,
                Sanphamchitiets = sanpham.Sanphamchitiets?
                    .Where(spct => spct.Trangthai != 2) // Lọc SPCT có trạng thái khác 2
                    .Select(spct => new SanphamchitietViewModel
                    {
                        Id = spct.Id,
                        Giathoidiemhientai = spct.Giathoidiemhientai,
                        TrangThai = spct.Trangthai,
                        Soluong = spct.Soluong,
                        IdChatLieu = spct.IdChatLieu,
                        IdMau = spct.IdMau,
                        IdSize = spct.IdSize,
                        UrlHinhanh = spct.UrlHinhanh,
                       
                        GiaSaleSanPhamChiTiet = spct.Salechitiets?
                            .Where(salect => salect.Sale != null && salect.Sale.Trangthai == 0 && salect.Soluong > 0) // Lọc Sale có trạng thái 0
                            .Select(salect =>
                                salect.Donvi == 0
                                    ? spct.Giathoidiemhientai - (decimal)salect.Giatrigiam // Giảm theo VND
                                    : spct.Giathoidiemhientai * (1 - (decimal)salect.Giatrigiam / 100m) // Giảm theo %
                            ).DefaultIfEmpty(spct.Giathoidiemhientai).Min(), // Lấy giá giảm nhỏ nhất
                        Sales = spct.Salechitiets?
                            .Where(salect => salect.Sale != null && salect.Sale.Trangthai == 0) // Chỉ lấy Sale đang diễn ra
                            .Select(salect => new SalechitietViewModel
                            {
                                Donvi = salect.Donvi,
                                Giatrigiam = salect.Giatrigiam,
                                Soluong = salect.Soluong
                            }).ToList() ?? new List<SalechitietViewModel>()
                    }).ToList() ?? new List<SanphamchitietViewModel>(),
            };

            return sanphamViewModel;
        }



        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamGiamGiaViewModels()
        {
            var sanphams = await _context.sanphams
        .Where(sp => sp.Trangthai != 2) // Lọc sản phẩm đang hoạt động
        .Select(sp => new
        {
            sp.Id,
            sp.TenSanpham,
            sp.Mota,
            sp.GiaBan,
            sp.NgayThemMoi,
            sp.Trangthai,
            sp.Soluong,
            ThuongHieu = sp.Thuonghieu != null ? sp.Thuonghieu.Tenthuonghieu : "N/A",
            sp.Idth,
            Sanphamchitiets = sp.Sanphamchitiets
                .Where(spct => spct.Trangthai == 0)
                .Where(spct => spct.Salechitiets.Any(sale => sale.Sale.Trangthai == 0 && sale.Soluong > 0)) // Chỉ lấy SPCT có giảm giá
                .Select(spct => new
                {
                    spct.Id,
                    spct.Giathoidiemhientai,
                    spct.Soluong,
                    spct.Trangthai,
                    Sales = spct.Salechitiets
                        .Where(sale => sale.Sale.Trangthai == 0) // Lọc sale đang hoạt động
                        .Select(sale => new
                        {
                            sale.Donvi,
                            sale.Giatrigiam,
                            sale.Sale.Ten,
                            GiaTriGiam = sale.Donvi == 0
                                ? (decimal)sale.Giatrigiam // Giảm theo VND
                                : spct.Giathoidiemhientai * ((decimal)sale.Giatrigiam / 100m), // Giảm theo %
                            GiaSaleSanPhamChiTiet = sale.Donvi == 0
                                ? spct.Giathoidiemhientai - (decimal)sale.Giatrigiam
                                : spct.Giathoidiemhientai * (1 - (decimal)sale.Giatrigiam / 100m)
                        })
                })
        })
        .Where(sp => sp.Sanphamchitiets.Any()) // Chỉ lấy sản phẩm có SPCT giảm giá
        .ToListAsync();

            // Xử lý sản phẩm
            var result = sanphams.Select(sp =>
            {
                var spctWithMaxSale = sp.Sanphamchitiets
                    .Select(spct => new
                    {
                        spct.Id,
                        spct.Giathoidiemhientai,
                        spct.Sales,
                        MaxSale = spct.Sales.OrderByDescending(sale => sale.GiaTriGiam).FirstOrDefault()
                    })
                    .OrderByDescending(spct => spct.MaxSale.GiaTriGiam)
                    .FirstOrDefault();

                var giaban = spctWithMaxSale != null
                    ? spctWithMaxSale.Giathoidiemhientai
                    : sp.GiaBan;

                return new SanphamViewModel
                {
                    Id = sp.Id,
                    Tensp = sp.TenSanpham,
                    Mota = sp.Mota,
                    Giaban = giaban,
                    Soluong = sp.Soluong,
                    NgayThemSanPham = sp.NgayThemMoi,
                    ThuongHieu = sp.ThuongHieu,
                    TrangThai = sp.Trangthai,
                    idThuongHieu = sp.Idth,
                    Giasale = spctWithMaxSale?.MaxSale?.GiaSaleSanPhamChiTiet ?? giaban,
                    GiaTriGiam = spctWithMaxSale?.MaxSale != null
                        ? (spctWithMaxSale.MaxSale.Donvi == 1
                       ? spctWithMaxSale.MaxSale.Giatrigiam // Giá trị giảm theo %
                       : spctWithMaxSale.MaxSale.Giatrigiam) // Giá trị giảm theo VND
                       : 0,

                };
            });

            return result;
        }

        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamByThuongHieu(int id)
        {
            var sanphams = await _context.sanphams
                .Where(sp => sp.Trangthai != 2 && sp.Idth == id)
                .Select(sp => new
                {
                    sp.Id,
                    sp.TenSanpham,
                    sp.Mota,
                    sp.GiaBan,
                    sp.NgayThemMoi,
                    sp.Trangthai,
                    sp.Soluong,
                    ThuongHieu = sp.Thuonghieu != null ? sp.Thuonghieu.Tenthuonghieu : "N/A",
                    sp.Idth,
                    Sanphamchitiets = sp.Sanphamchitiets
                    .Where(spct => spct.Trangthai == 0)
                 .Select(spct => new
                 {
                     spct.Id,
                     spct.Giathoidiemhientai,
                     spct.Soluong,
                     Sales = spct.Salechitiets
                         .Where(sale => sale.Sale.Trangthai == 0 && sale.Soluong > 0) // Chỉ lấy sale đang hoạt động
                         .Select(sale => new
                         {
                             sale.Donvi,
                             sale.Giatrigiam,
                             sale.Sale.Ten,
                             GiaTriGiam = sale.Donvi == 0
                                 ? (decimal)sale.Giatrigiam // Giảm theo VND
                                 : spct.Giathoidiemhientai * ((decimal)sale.Giatrigiam / 100m), // Giảm theo %
                             GiaSaleSanPhamChiTiet = sale.Donvi == 0
                                 ? spct.Giathoidiemhientai - (decimal)sale.Giatrigiam
                                 : spct.Giathoidiemhientai * (1 - (decimal)sale.Giatrigiam / 100m)
                         })
                 })
                })
         .ToListAsync();

            // Xử lý sản phẩm với giá bán và giá giảm
            var result = sanphams.Select(sp =>
            {
                // Tìm sản phẩm chi tiết có giảm giá lớn nhất
                var spctWithMaxSale = sp.Sanphamchitiets
                    .Select(spct => new
                    {
                        spct.Id,
                        spct.Giathoidiemhientai,
                        spct.Sales,
                        MaxSale = spct.Sales.OrderByDescending(sale => sale.GiaTriGiam).FirstOrDefault()
                    })
                    .Where(spct => spct.MaxSale != null)

                    .OrderByDescending(spct => spct.MaxSale.GiaTriGiam)
                    .FirstOrDefault();

                // Tính giá bán
                var giaban = spctWithMaxSale != null
                    ? spctWithMaxSale.Giathoidiemhientai // Giá của spct được giảm giá nhiều nhất
                    : sp.Sanphamchitiets.Any()
                        ? sp.Sanphamchitiets.Min(spct => spct.Giathoidiemhientai) // Giá nhỏ nhất trong spct
                        : sp.GiaBan; // Nếu không có spct, lấy giá sản phẩm gốc

                return new SanphamViewModel
                {
                    Id = sp.Id,
                    Tensp = sp.TenSanpham,
                    Mota = sp.Mota,
                    Giaban = giaban,
                    TrangThai = sp.Trangthai,
                    Soluong = sp.Soluong,
                    NgayThemSanPham = sp.NgayThemMoi,
                    ThuongHieu = sp.ThuongHieu,
                    idThuongHieu = sp.Idth,
                    Giasale = spctWithMaxSale?.MaxSale?.GiaSaleSanPhamChiTiet ?? giaban,
                    GiaTriGiam = spctWithMaxSale?.MaxSale != null
                        ? (spctWithMaxSale.MaxSale.Donvi == 1
                       ? spctWithMaxSale.MaxSale.Giatrigiam // Giá trị giảm theo %
                       : spctWithMaxSale.MaxSale.Giatrigiam) // Giá trị giảm theo VND
                       : 0,

                };
            });

            return result;
        }



        public async Task<IEnumerable<SanphamViewModel>> GetSanphamByThuocTinh(
    
    decimal? giaMin = null,
    decimal? giaMax = null,
    int? idThuongHieu = null)
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var sanphams = await _context.sanphams
                .Where(sp => sp.Trangthai != 2) // Lọc sản phẩm đang hoạt động
                .Select(sp => new
                {
                    sp.Id,
                    sp.TenSanpham,
                    sp.Mota,
                    sp.GiaBan,
                    sp.NgayThemMoi,
                    sp.Soluong,
                    sp.Trangthai,
                    ThuongHieu = sp.Thuonghieu != null ? sp.Thuonghieu.Tenthuonghieu : "N/A",
                    sp.Idth,
                    Sanphamchitiets = sp.Sanphamchitiets
                        .Where(spct => spct.Trangthai != 2)
                      .Select(spct => new
                      {
                          spct.Id,
                          spct.Giathoidiemhientai,
                          spct.Soluong,
                          spct.Color,
                          spct.Size,
                          spct.ChatLieu,
                          Sales = spct.Salechitiets
                                 .Where(sale => sale.Sale.Trangthai == 0 && sale.Soluong > 0) // Chỉ lấy sale đang hoạt động
                                 .Select(sale => new
                                 {
                                     sale.Donvi,
                                     sale.Giatrigiam,
                                     sale.Sale.Ten,
                                     GiaTriGiam = sale.Donvi == 0
                                         ? (decimal)sale.Giatrigiam // Giảm theo VND
                                         : spct.Giathoidiemhientai * ((decimal)sale.Giatrigiam / 100m), // Giảm theo %
                                     GiaSaleSanPhamChiTiet = sale.Donvi == 0
                                         ? spct.Giathoidiemhientai - (decimal)sale.Giatrigiam
                                         : spct.Giathoidiemhientai * (1 - (decimal)sale.Giatrigiam / 100m)
                                 })
                      })
                })
                .ToListAsync();

            // Áp dụng logic xử lý để tính giá bán và giá giảm
            var result = sanphams
                .Select(sp =>
                {
                    // Tìm sản phẩm chi tiết có giảm giá lớn nhất
                    var spctWithMaxSale = sp.Sanphamchitiets
                        .Select(spct => new
                        {
                            spct.Id,
                            spct.Giathoidiemhientai,
                            spct.Sales,
                            MaxSale = spct.Sales.OrderByDescending(sale => sale.GiaTriGiam).FirstOrDefault()
                        })
                        .Where(spct => spct.MaxSale != null)
                        .OrderByDescending(spct => spct.MaxSale.GiaTriGiam)
                        .FirstOrDefault();

                    // Tính giá bán
                    var giaban = spctWithMaxSale != null
                        ? spctWithMaxSale.Giathoidiemhientai
                        : sp.Sanphamchitiets.Any()
                            ? sp.Sanphamchitiets.Min(spct => spct.Giathoidiemhientai)
                            : sp.GiaBan;

                    return new SanphamViewModel
                    {
                        Id = sp.Id,
                        Tensp = sp.TenSanpham,
                        Mota = sp.Mota,
                        Giaban = giaban,
                        Soluong = sp.Soluong,
                        NgayThemSanPham = sp.NgayThemMoi,
                        TrangThai = sp.Trangthai,
                        ThuongHieu = sp.ThuongHieu,
                        idThuongHieu = sp.Idth,
                        Giasale = spctWithMaxSale?.MaxSale?.GiaSaleSanPhamChiTiet ?? giaban,
                        GiaTriGiam = spctWithMaxSale?.MaxSale != null
                            ? (spctWithMaxSale.MaxSale.Donvi == 1
                            ? spctWithMaxSale.MaxSale.Giatrigiam // Giá trị giảm theo %
                            : spctWithMaxSale.MaxSale.Giatrigiam) // Giá trị giảm theo VND
                            : 0,
                    };
                });

            // Lọc theo giá bán
            if (giaMin.HasValue)
            {
                result = result.Where(sp => sp.Giasale >= giaMin.Value);
            }

            if (giaMax.HasValue)
            {
                result = result.Where(sp => sp.Giasale <= giaMax.Value);
            }

            // Lọc theo thương hiệu
            if (idThuongHieu.HasValue)
            {
                result = result.Where(sp => sp.idThuongHieu == idThuongHieu.Value);
            }

            return result.ToList();
        }

        public async Task<List<Sanpham>> GetListByIdsAsync(List<int> ids)
        {
            var sanphams = await _context.sanphams
                .Where(dg => ids.Contains(dg.Id))
                .ToListAsync();

            return sanphams;
        }
    }
}
