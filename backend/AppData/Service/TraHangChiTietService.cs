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
    public class TraHangChiTietService : ITraHangChiTietService

    {
        private readonly ITraHangChiTietRepo _repos;
        private readonly IHoaDonChiTietRepo _HDCTrepos;
        private readonly ITraHangRepo _THrepos;
        public TraHangChiTietService(ITraHangChiTietRepo repos, IHoaDonChiTietRepo hDCTrepos, ITraHangRepo tHrepos)
        {
            _repos = repos;
            _HDCTrepos = hDCTrepos;
            _THrepos = tHrepos;
        }

        public async Task Add(TrahangchitietDTO ct)
        {
            // Kiểm tra nếu trà hàng không tồn tại
            var trahang = await _THrepos.GetById(ct.Idth);
            if (trahang == null)
                throw new ArgumentNullException("Trà hàng không tồn tại");

            // Kiểm tra nếu trà hàng không tồn tại
            var hdct = await _HDCTrepos.GetByIdAsync(ct.Idhdct);
            if (hdct == null)
                throw new ArgumentNullException("Hoá đơn chi tiết không tồn tại");

            var a = new Trahangchitiet
            {
                Idth = ct.Idth,
                Soluong = ct.Soluong,
                Tinhtrang = ct.Tinhtrang,
                Idhdct = ct.Idhdct
            };
            await _repos.Add(a);

            ct.Id = a.Id;
        }

        public async Task Delete(int id)
        {
            await _repos.Delete(id);
        }

        public async Task<List<TrahangchitietDTO>> GetAll()
        {
            var a = await _repos.GetAll();
            return a.Select(x => new TrahangchitietDTO
            {
                Id = x.Id,
                Idth = x.Idth,
                Soluong = x.Soluong,
                Tinhtrang = x.Tinhtrang,
                Idhdct = x.Idhdct
            }).ToList();
        }

        public async Task<TrahangchitietDTO> GetById(int id)
        {
            var x = await _repos.GetById(id);
            return new TrahangchitietDTO
            {
                Id = x.Id,
                Idth = x.Idth,
                Soluong = x.Soluong,
                Tinhtrang = x.Tinhtrang,
                Idhdct = x.Idhdct
            };
        }

        public async Task<List<HoadonchitietViewModel>> ListSanPhamByIdhd(int id)
        {
            return await _repos.ListSanPhamByIdhd(id);
        }

        public async Task<List<TraHangchitietViewModel>> ListSanPhamByIdth(int id)
        {

            try
            {
                var a = await _repos.ListSanPhamByIdth(id);
                return a;
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }

        public async Task<List<SanPhamTraHang>> SanphamByThct()
        {

            try
            {
                return await _repos.SanphamByThct();
            }
            catch (Exception ex)
            {
                throw new KeyNotFoundException(ex.Message);
            }
        }
        public async Task Update(int id, TrahangchitietDTO ct)
        {

            // Kiểm tra nếu trà hàng không tồn tại
            var a = await _repos.GetById(id);
            if (a == null)
                throw new ArgumentNullException("Trà hàng không tồn tại");
            // Kiểm tra nếu trà hàng không tồn tại
            var trahang = await _THrepos.GetById(ct.Idth);
            if (trahang == null)
                throw new ArgumentNullException("Trà hàng không tồn tại");

            // Kiểm tra nếu trà hàng không tồn tại
            var hdct = await _THrepos.GetById(ct.Idhdct);
            if (hdct == null)
                throw new ArgumentNullException("Hoá đơn chi tiết không tồn tại");

            if (a != null)
            {
                a.Idth = ct.Idth;
                a.Soluong = ct.Soluong;
                a.Tinhtrang = ct.Tinhtrang;
                a.Idhdct = ct.Idhdct;
                await _repos.Update(a);
            }
            else
            {
                throw new KeyNotFoundException("Không tồn tại!");
            }
        }
    }
}
