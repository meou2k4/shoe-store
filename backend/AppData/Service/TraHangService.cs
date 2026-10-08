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
    public class TraHangService : ITraHangService
    {
        private readonly ITraHangRepo _repos;
        private readonly IHoaDonRepo _HDrepos;
        private readonly ITraHangChiTietRepo _ThChiTietRepo;
        public TraHangService(ITraHangRepo repos, IHoaDonRepo hDrepos, ITraHangChiTietRepo traHangChiTietRepo)
        {
            _repos = repos;
            _ThChiTietRepo = traHangChiTietRepo;
            _HDrepos = hDrepos;
        }
        public async Task Add(TrahangtaoDTO trahang)
        {
            var a = new Trahang
            {
                Tenkhachhang = trahang.Tenkhachhang,
                Idkh = trahang.Idkh,
                Idnv = (trahang.Idnv != null && trahang.Idnv != 0) ? trahang.Idnv : (int?)null,
                Sotienhoan = trahang.Sotienhoan,
                Lydotrahang = trahang.Lydotrahang != null ? trahang.Lydotrahang : null,
                Trangthai = trahang.Trangthai,
                Phuongthuchoantien = trahang.Phuongthuchoantien,
                Chuthich = trahang.Chuthich != null ? trahang.Chuthich : null,
                Tennganhang = trahang.Tennganhang != null ? trahang.Tennganhang : null,
                Sotaikhoan = trahang.Sotaikhoan != null ? trahang.Sotaikhoan : null,
                Tentaikhoan = trahang.Tentaikhoan != null ? trahang.Tentaikhoan : null,
                Hinhthucxuly = trahang.Hinhthucxuly != null ? trahang.Hinhthucxuly : null,
                Ngaytrahangthucte = trahang.Ngaytrahangthucte != null ? trahang.Ngaytrahangthucte : null,
                Diachiship = trahang.Diachiship,
                Trangthaihoantien = trahang.Trangthaihoantien,
                Ngaytaodon = trahang.Ngaytaodon
            };
            await _repos.Add(a);

            trahang.Id = a.Id;
        }

        public async Task DeleteById(int id)
        {
            await _repos.DeleteById(id);
        }

        public async Task<List<TrahangDTO>> GetAll()
        {
            var a = await _repos.GetAll();
            return a;
        }
        public async Task<TrahangDTO> GetById1(int id)
        {
            var a = await _repos.GetById1(id);
            return a;
        }

        public async Task<TrahangDTO> GetById(int id)
        {
            var x = await _repos.GetById(id);
            return new TrahangDTO
            {
                Id = x.Id,
                Tenkhachhang = x.Tenkhachhang,
                Idkh = x.Idkh,
                Idnv = x.Idnv,
                Sotienhoan = x.Sotienhoan,
                Lydotrahang = x.Lydotrahang,
                Trangthai = x.Trangthai,
                Phuongthuchoantien = x.Phuongthuchoantien,
                Chuthich = x.Chuthich,
                Tennganhang = x.Tennganhang,
                Sotaikhoan = x.Sotaikhoan,
                Tentaikhoan = x.Tentaikhoan,
                Hinhthucxuly = x.Hinhthucxuly,
                Diachiship = x.Diachiship,
                Ngaytrahangthucte = x.Ngaytrahangthucte,
                Trangthaihoantien = x.Trangthaihoantien,
                Ngaytaodon = x.Ngaytaodon
            };
        }
        public async Task UpdateTrangThaiTh(int id, string? chuthich, int trangthai, int idnv)
        {
            try
            {
                var a = await _repos.GetById(id);
                if (a == null)
                {
                    throw new KeyNotFoundException("Không tồn tại!");
                }
                else
                {
                    a.Idnv = idnv;
                    a.Chuthich = chuthich;
                    a.Trangthai = trangthai;
                    await _repos.Update(a);
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }

        public async Task Update(int id, TrahangtaoDTO trahang)
        {
            var a = await _repos.GetById(id);
            if (a == null)
            {
                throw new KeyNotFoundException("Không tồn tại!");
            }
            else
            {
                a.Tenkhachhang = trahang.Tenkhachhang;
                a.Idkh = trahang.Idkh;
                a.Idnv = trahang.Idnv;
                a.Sotienhoan = trahang.Sotienhoan;
                a.Lydotrahang = trahang.Lydotrahang;
                a.Trangthai = trahang.Trangthai;
                a.Phuongthuchoantien = trahang.Phuongthuchoantien;
                a.Chuthich = trahang.Chuthich;
                a.Tennganhang = BCrypt.Net.BCrypt.HashPassword(trahang.Tennganhang) != null ? trahang.Tennganhang : null;
                a.Sotaikhoan = BCrypt.Net.BCrypt.HashPassword(trahang.Sotaikhoan) != null ? trahang.Sotaikhoan : null;
                a.Tentaikhoan = BCrypt.Net.BCrypt.HashPassword(trahang.Tentaikhoan) != null ? trahang.Tentaikhoan : null;
                a.Hinhthucxuly = trahang.Hinhthucxuly;
                a.Diachiship = trahang.Diachiship;
                a.Ngaytrahangthucte = trahang.Ngaytrahangthucte != null ? trahang.Ngaytrahangthucte : null;
                a.Trangthaihoantien = trahang.Trangthaihoantien;
                a.Ngaytaodon = trahang.Ngaytaodon;
                await _repos.Update(a);
            }
        }

        public async Task UpdateTrangThaiHd(int id)
        {
            try
            {
                var a = await _HDrepos.GetByIdAsync(id);
                if (a == null)
                {
                    throw new KeyNotFoundException("Không tồn tại!");
                }
                else
                {
                    a.Trangthaidonhang = 5;
                    await _HDrepos.UpdateAsync(a);
                }
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }
        
        public async Task Huydon(int id, int idnv, string? chuthich)
        {
            try
            {
                var a = await _repos.GetById(id);
                if(a == null)
                {
                    throw new KeyNotFoundException("Không tồn tại hóa đơn!");
                }
                a.Idnv = idnv;
                a.Chuthich = chuthich;
                a.Trangthai = 2;
                await _repos.Update(a);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }

        public async Task XacNhan(int id, string hinhthucxuly, int idnv, string? chuthich)
        {
            try
            {
                await _repos.XacNhan(id, hinhthucxuly, idnv, chuthich);
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }
    }
}
