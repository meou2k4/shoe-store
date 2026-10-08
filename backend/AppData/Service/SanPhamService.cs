using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using AppData.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepo _repository;
        private readonly IHoaDonChiTietRepo _hoaDonChiTietRepo;
        private readonly ISanPhamChiTietRepo _sanPhamChiTietRepo;
        private readonly IDanhGiaRepo _danhGiaRepo;
        private readonly IThuongHieuRepo _thuonghieuRepo;

        public SanPhamService(IThuongHieuRepo thuonghieuRepo,ISanPhamRepo repository, IHoaDonChiTietRepo hoaDonChiTietRepo, ISanPhamChiTietRepo sanPhamChiTietRepo, IDanhGiaRepo danhGiaRepo)
        {
            _repository = repository;
            _hoaDonChiTietRepo = hoaDonChiTietRepo;
            _sanPhamChiTietRepo = sanPhamChiTietRepo;
            _danhGiaRepo = danhGiaRepo;
            _thuonghieuRepo = thuonghieuRepo;
        }

        public async Task<IEnumerable<SanphamDTO>> GetAllAsync()
        {
            var sanphams = await _repository.GetAllAsync();
            var sanphamchitiets = await _sanPhamChiTietRepo.GetAllAsync();

            if (sanphams == null || !sanphams.Any())
            {
                throw new Exception("Không có sản phẩm nào trong danh sách.");
            }

            var sanphamDTOs = new List<SanphamDTO>();

            foreach (var item in sanphams)
            {
                var chiTiets = sanphamchitiets.Where(spct => spct.Idsp == item.Id).ToList();

                var tongSoLuong = chiTiets
                    .Sum(spct => spct.Soluong);
                if (item.Trangthai == 2)
                {
                    foreach (var spct in chiTiets)
                    {
                        if (spct.Trangthai != 2)
                        {
                            spct.Trangthai = 2;
                            await _sanPhamChiTietRepo.UpdateAsync(spct);
                        }
                    }
                }
     
                item.Soluong = tongSoLuong;

                if (item.Soluong <= 0 && item.Trangthai != 1 && item.Trangthai != 2 && item.Trangthai != 3)
                {
                    item.Trangthai = 1;
                }
                if (item.Soluong > 0 && item.Trangthai != 0 && item.Trangthai != 2 && item.Trangthai != 3)
                {
                    item.Trangthai = 0;
                }
               

                await _repository.UpdateAsync(item);

                sanphamDTOs.Add(new SanphamDTO()
                {
                    Id = item.Id,
                    TenSanpham = item.TenSanpham,
                    Mota = item.Mota,
                    Trangthai = item.Trangthai,
                    Chieucao = item.Chieucao,
                    Soluong = item.Soluong,
                    GiaBan = item.GiaBan,
                    NgayThemMoi = item.NgayThemMoi,
                    Chieudai = item.Chieudai,
                    Chieurong = item.Chieurong,
                    Trongluong = item.Trongluong,
                    Idth = item.Idth
                });
            }

            return sanphamDTOs;
        }

        public async Task AddAsync(SanphamDTO sanphamDto)
        {
            var sanpham = new Sanpham
            {
                TenSanpham = sanphamDto.TenSanpham,
                Mota = sanphamDto.Mota,
                Soluong = sanphamDto.Soluong,
                GiaBan = sanphamDto.GiaBan,
                Chieudai = sanphamDto.Chieudai,
                Chieurong = sanphamDto.Chieurong,
                Chieucao = sanphamDto.Chieucao,
                Trongluong = sanphamDto.Trongluong,
                NgayThemMoi = DateTime.Now,
                //Giasale = sanphamDto.Giasale,
                Idth = sanphamDto.Idth,
                Trangthai = sanphamDto.Trangthai
            };

            await _repository.AddAsync(sanpham);
            sanphamDto.Id = sanpham.Id;
        }

        public async Task UpdateAsync(int id, SanphamDTO sanphamDto)
        {
            var sanpham = await _repository.GetByIdAsync(id);
            if (sanpham == null) return;

            sanpham.TenSanpham = sanphamDto.TenSanpham;
            sanpham.Mota = sanphamDto.Mota;
            sanpham.Soluong = sanphamDto.Soluong;
            sanpham.GiaBan = sanphamDto.GiaBan;
            sanpham.Chieudai = sanphamDto.Chieudai;
            sanpham.Chieurong = sanphamDto.Chieurong;
            sanpham.Chieucao = sanphamDto.Chieucao;
            sanpham.Trongluong = sanphamDto.Trongluong;
            //sanpham.Giasale = sanphamDto.Giasale;
            sanpham.Idth = sanphamDto.Idth;
            sanpham.Trangthai = sanphamDto.Trangthai ;

            await _repository.UpdateAsync(sanpham);
            sanpham.Idth = sanphamDto.Id;
        }

        public async Task DeleteAsync(int id)
        {
           await _repository.DeleteAsync(id);

        }

        public async Task<IEnumerable<SanphamDTO>> SearchByNameAsync(string name)
        {
            var sanphams = await _repository.SearchByNameAsync(name);
            return sanphams.Select(sp => new SanphamDTO
            {
                Id = sp.Id,
                TenSanpham = sp.TenSanpham,
                Mota = sp.Mota,
                Trangthai = sp.Trangthai,
                Soluong = sp.Soluong,
                GiaBan = sp.GiaBan,
                //Giasale = sp.Giasale,
                Chieudai = sp.Chieudai,
                Chieucao = sp.Chieucao,
                Chieurong = sp.Chieurong,
                Trongluong = sp.Trongluong,
                Idth = sp.Idth
            });
        }
        public async Task UpdateStatusLoad(int id)
        {

            var sale = await _repository.GetByIdAsync(id);
           

            if (sale == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại");
            }
            var sanphamchitiets = (await _sanPhamChiTietRepo.GetAllAsync())
             .Where(spct => spct.Idsp == id)
             .ToList();

            if (sale.Trangthai != 3)
            {
                
                sale.Trangthai = 0; // Đang diễn ra
                  foreach (var item in sanphamchitiets)
                  {
                   item.Trangthai = 0;
                   await _sanPhamChiTietRepo.UpdateAsync(item);

                  }
            }


            await _repository.UpdateAsync(sale);
        }
        public async Task UpdateStatusToCancelled(int id)
        {
            var sale = await _repository.GetByIdAsync(id);
            if (sale == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại");
            }

            sale.Trangthai = 2; // Cập nhật trạng thái thành Hủy
            await _repository.UpdateAsync(sale);
        }
        public async Task<SanphamDTO> GetByIdAsync(int id)
        {
            var sanpham = await _repository.GetByIdAsync(id);

            if (sanpham == null)
                return null;

            // Chuyển đổi đối tượng Sanpham thành SanphamDTO
            return new SanphamDTO
            {
                Id = sanpham.Id,
                TenSanpham = sanpham.TenSanpham,
                Mota = sanpham.Mota,
                Trangthai = sanpham.Trangthai,
                Soluong = sanpham.Soluong,
                GiaBan = sanpham.GiaBan,
                //Giasale = sanpham.Giasale,
                Chieudai = sanpham.Chieudai,
                Chieucao = sanpham.Chieucao,
                Chieurong = sanpham.Chieurong,
                Trongluong = sanpham.Trongluong,
                Idth = sanpham.Idth
            };
        }

        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamViewModels()
        {
            var sanPham = await _repository.GetAllSanphamViewModels();

            foreach (var item in sanPham)
            {
                var DiemDG = await TinhTrungBinhDanhGia(item.Id);
                item.trungBinhDanhGia = DiemDG;
                foreach (var item1 in item.Sanphamchitiets)
                {
                    item1.soLuongBan = await GetTotalSoldQuantityAsync(item1.Id);
                }
            }

        
            var result = sanPham.Select(sp =>
            {
                // Danh sách sản phẩm chi tiết kèm mức giảm giá cao nhất
                var spctWithMaxSale = sp.Sanphamchitiets
                 .Select(spct => new
                 {
                 spct.Id,
                 spct.Giathoidiemhientai,
                 spct.GiaSaleSanPhamChiTiet,
                 spct.Sales,
                 MaxSale = spct.Sales
                 .Where(s => s.trangThai == 0 && s.Ngaybatdau <= DateTime.Now && s.Ngayketthuc >= DateTime.Now && s.Soluong > 0)
                 .OrderByDescending(s => s.Donvi == 1
                 ? s.Giatrigiam * spct.Giathoidiemhientai / 100m
                 : s.Giatrigiam)
                 .FirstOrDefault()
                 })
                .Where(spct => spct.MaxSale != null)
                .OrderByDescending(spct => spct.MaxSale.Donvi == 1
                ? spct.MaxSale.Giatrigiam * spct.Giathoidiemhientai / 100m
                : spct.MaxSale.Giatrigiam)
                .FirstOrDefault();
              

                return new SanphamViewModel
                {
                    Id = sp.Id,
                    Tensp = sp.Tensp,
                    Mota = sp.Mota,
                    Giaban = spctWithMaxSale?.Giathoidiemhientai ?? sp.Giaban,
                    TrangThai = sp.TrangThai,
                    Soluong = sp.Soluong,
                    NgayThemSanPham = sp.NgayThemSanPham,
                    ThuongHieu = sp.ThuongHieu,
                    idThuongHieu = sp.idThuongHieu,
                    trungBinhDanhGia = sp.trungBinhDanhGia,
                    Giasale = spctWithMaxSale?.GiaSaleSanPhamChiTiet ?? sp.Giaban,
                    GiaTriGiam = spctWithMaxSale?.MaxSale?.Giatrigiam ?? 0,
                    Sanphamchitiets = sp.Sanphamchitiets.Select(spct => new SanphamchitietViewModel
                    {
                        Id = spct.Id,
                        Giathoidiemhientai = spct.Giathoidiemhientai,
                        GiaSaleSanPhamChiTiet = spct.GiaSaleSanPhamChiTiet,
                        Soluong = spct.Soluong,
                        TrangThai = spct.TrangThai,
                        UrlHinhanh = spct.UrlHinhanh,
                        soLuongBan = spct.soLuongBan,
                        IdSize = spct.IdSize,
                        IdChatLieu = spct.IdChatLieu,
                        IdMau = spct.IdMau,
                        Sales = spct.Sales.Select(s => new SalechitietViewModel
                        {
                            trangThai = s.trangThai,
                            Donvi = s.Donvi,
                            Giatrigiam = s.Giatrigiam,
                            Soluong = s.Soluong,
                            TenSale = s.TenSale,
                            Ngaybatdau = s.Ngaybatdau,
                            Ngayketthuc = s.Ngayketthuc
                        }).ToList()
                    }).ToList()

                };
            });

            return result;
        }


        public async Task<SanphamViewModel> GetAllSanphamViewModelsByIdSP(int idsp)
        {
           
            var sanPham = await _repository.GetSanphamViewModelByIdSP(idsp);

        
            foreach (var item in sanPham.Sanphamchitiets)
            {
                item.soLuongBan = await GetTotalSoldQuantityAsync(item.Id);
            }

          
            var spctWithMaxSale = sanPham.Sanphamchitiets
                .Select(spct => new
                {
                    spct.Id,
                    spct.Giathoidiemhientai,
                    spct.GiaSaleSanPhamChiTiet,
                    spct.Sales,
                    MaxSale = spct.Sales
                        .Where(s => s.trangThai == 0 && s.Ngaybatdau <= DateTime.Now && s.Ngayketthuc >= DateTime.Now && s.Soluong > 0)
                        .OrderByDescending(s => s.Donvi == 1
                            ? s.Giatrigiam * spct.Giathoidiemhientai / 100m
                            : s.Giatrigiam)
                        .FirstOrDefault()
                })
                .Where(spct => spct.MaxSale != null)
                .OrderByDescending(spct => spct.MaxSale.Donvi == 1
                    ? spct.MaxSale.Giatrigiam * spct.Giathoidiemhientai / 100m
                    : spct.MaxSale.Giatrigiam)
                .FirstOrDefault();

            
            var result = new SanphamViewModel
            {
                Id = sanPham.Id,
                Tensp = sanPham.Tensp,
                Mota = sanPham.Mota,
                Giaban = spctWithMaxSale?.Giathoidiemhientai ?? sanPham.Giaban,
                TrangThai = sanPham.TrangThai,
                Soluong = sanPham.Soluong,
                NgayThemSanPham = sanPham.NgayThemSanPham,
                ThuongHieu = sanPham.ThuongHieu,
                idThuongHieu = sanPham.idThuongHieu,
                trungBinhDanhGia = sanPham.trungBinhDanhGia,
                Giasale = spctWithMaxSale?.GiaSaleSanPhamChiTiet ?? sanPham.Giaban,
                GiaTriGiam = spctWithMaxSale?.MaxSale?.Giatrigiam ?? 0,
                Sanphamchitiets = sanPham.Sanphamchitiets.Select(spct => new SanphamchitietViewModel
                {
                    Id = spct.Id,
                    Giathoidiemhientai = spct.Giathoidiemhientai,
                    GiaSaleSanPhamChiTiet = spct.GiaSaleSanPhamChiTiet,
                    Soluong = spct.Soluong,
                    TrangThai = spct.TrangThai,
                    UrlHinhanh = spct.UrlHinhanh,
                    soLuongBan = spct.soLuongBan,
                    IdSize = spct.IdSize,
                    IdChatLieu = spct.IdChatLieu,
                    IdMau = spct.IdMau,
                    Sales = spct.Sales.Select(s => new SalechitietViewModel
                    {
                        trangThai = s.trangThai,
                        Donvi = s.Donvi,
                        Giatrigiam = s.Giatrigiam,
                        Soluong = s.Soluong,
                        TenSale = s.TenSale,
                        Ngaybatdau = s.Ngaybatdau,
                        Ngayketthuc = s.Ngayketthuc
                    }).ToList()
                }).ToList()
            };

            return result;
        }

        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamGiamGiaViewModels()
        {
            var sanPham = await _repository.GetAllSanphamViewModels();

            foreach (var item in sanPham)
            {
                var DiemDG = await TinhTrungBinhDanhGia(item.Id);
                item.trungBinhDanhGia = DiemDG;
                foreach (var item1 in item.Sanphamchitiets)
                {
                    item1.soLuongBan = await GetTotalSoldQuantityAsync(item1.Id);
                }
            }


            var result = sanPham.Select(sp =>
            {
                // Danh sách sản phẩm chi tiết kèm mức giảm giá cao nhất
                var spctWithMaxSale = sp.Sanphamchitiets
                 .Select(spct => new
                 {
                     spct.Id,
                     spct.Giathoidiemhientai,
                     spct.GiaSaleSanPhamChiTiet,
                     spct.Sales,
                     MaxSale = spct.Sales
                 .Where(s => s.trangThai == 0 && s.Ngaybatdau <= DateTime.Now && s.Ngayketthuc >= DateTime.Now && s.Soluong > 0)
                 .OrderByDescending(s => s.Donvi == 1
                 ? s.Giatrigiam * spct.Giathoidiemhientai / 100m
                 : s.Giatrigiam)
                 .FirstOrDefault()
                 })
                .Where(spct => spct.MaxSale != null)
                .OrderByDescending(spct => spct.MaxSale.Donvi == 1
                ? spct.MaxSale.Giatrigiam * spct.Giathoidiemhientai / 100m
                : spct.MaxSale.Giatrigiam)
                .FirstOrDefault();


                return new SanphamViewModel
                {
                    Id = sp.Id,
                    Tensp = sp.Tensp,
                    Mota = sp.Mota,
                    Giaban = spctWithMaxSale?.Giathoidiemhientai ?? sp.Giaban,
                    TrangThai = sp.TrangThai,
                    Soluong = sp.Soluong,
                    NgayThemSanPham = sp.NgayThemSanPham,
                    ThuongHieu = sp.ThuongHieu,
                    idThuongHieu = sp.idThuongHieu,
                    trungBinhDanhGia = sp.trungBinhDanhGia,
                    Giasale = spctWithMaxSale?.GiaSaleSanPhamChiTiet ?? sp.Giaban,
                    GiaTriGiam = spctWithMaxSale?.MaxSale?.Giatrigiam ?? 0,
                    Sanphamchitiets = sp.Sanphamchitiets.Select(spct => new SanphamchitietViewModel
                    {
                        Id = spct.Id,
                        Giathoidiemhientai = spct.Giathoidiemhientai,
                        GiaSaleSanPhamChiTiet = spct.GiaSaleSanPhamChiTiet,
                        Soluong = spct.Soluong,
                        TrangThai = spct.TrangThai,
                        UrlHinhanh = spct.UrlHinhanh,
                        soLuongBan = spct.soLuongBan,
                        IdSize = spct.IdSize,
                        IdChatLieu = spct.IdChatLieu,
                        IdMau = spct.IdMau,
                        Sales = spct.Sales.Select(s => new SalechitietViewModel
                        {
                            trangThai = s.trangThai,
                            Donvi = s.Donvi,
                            Giatrigiam = s.Giatrigiam,
                            Soluong = s.Soluong,
                            TenSale = s.TenSale,
                            Ngaybatdau = s.Ngaybatdau,
                            Ngayketthuc = s.Ngayketthuc
                        }).ToList()
                    }).ToList()

                };
            })
            .Where(sp => sp.Giasale < sp.Giaban)
            .ToList();

            // Kiểm tra nếu không có sản phẩm nào giảm giá
            if (!result.Any())
            {
                throw new Exception("Không có sản phẩm khuyến mãi.");
            }

            return result;
        }

        public async Task<IEnumerable<SanphamViewModel>> GetAllSanphamByThuongHieu(int id)
        {
            return await _repository.GetAllSanphamByThuongHieu(id);
        }

        public async Task<IEnumerable<SanphamViewModel>> GetSanphamByThuocTinh(
       decimal? giaMin = null,
       decimal? giaMax = null,
       List<int> idThuongHieu = null,
       List<int> idSize = null,
       bool? coSale = null)
        {
            var sanPhams = await GetAllSanphamViewModels();

            var filteredSanPhams = sanPhams.Where(sp => sp.TrangThai != 3);

            if (giaMin.HasValue)
            {
                filteredSanPhams = filteredSanPhams.Where(sp => sp.Giaban >= giaMin.Value);
            }

            if (giaMax.HasValue)
            {
                filteredSanPhams = filteredSanPhams.Where(sp => sp.Giaban <= giaMax.Value);
            }

            if (idThuongHieu != null && idThuongHieu.Any())
            {
                filteredSanPhams = filteredSanPhams.Where(sp => idThuongHieu.Contains(sp.idThuongHieu));
            }

            if (idSize != null && idSize.Any())
            {
                filteredSanPhams = filteredSanPhams.Where(sp => sp.Sanphamchitiets.Any(spct => idSize.Contains(spct.IdSize)));
            }

            if (coSale.HasValue)
            {
                if (coSale.Value)
                {
                    filteredSanPhams = filteredSanPhams.Where(sp => sp.Giasale < sp.Giaban);
                }
                else
                {
                    filteredSanPhams = filteredSanPhams.Where(sp => sp.Giasale == sp.Giaban);
                }
            }

            return filteredSanPhams.ToList();
        }


        public async Task<int> GetTotalSoldQuantityAsync(int idSanphamChitiet)
        {
            var hoaDonChiTiets = await _hoaDonChiTietRepo.GetAllAsync();

            return hoaDonChiTiets
            .Where(hdct => hdct.Idspct == idSanphamChitiet &&
                     hdct.Hoadon.Trangthai == 0 && hdct.Hoadon.Trangthaidonhang == 3)
            .Select(hdct => hdct.Soluong)
            .DefaultIfEmpty(0) 
            .Sum();

        }
        public async Task<float> TinhTrungBinhDanhGia(int idSanpham)
        {
            var danhgiaList = await _danhGiaRepo.GetByidSP(idSanpham);

            if (danhgiaList == null || danhgiaList.Count == 0) // Kiểm tra null hoặc rỗng
            {
                return 0;
            }

            float sum = 0;
            foreach (var item in danhgiaList)
            {
                sum += item.Sosao;
            }

            float TB = sum / danhgiaList.Count;
            return (float)Math.Round(TB, 1); // Làm tròn đến 1 chữ số thập phân
        }

        public async Task<List<Sanpham>> GetListByIdsAsync(List<int> ids)
        {
            return await _repository.GetListByIdsAsync(ids);
        }

    }
}
