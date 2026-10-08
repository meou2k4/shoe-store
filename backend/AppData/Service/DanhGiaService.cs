using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using AppData.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly IDanhGiaRepo _repos;
        private readonly IHinhAnhRepo _hinhAnhRepo;

        public DanhGiaService(IDanhGiaRepo repos, IHinhAnhRepo hinhAnhRepo)
        {
            _repos = repos;
            _hinhAnhRepo = hinhAnhRepo;
        }

        public async Task Create(DanhgiaDTO danhGiaDTO)
        {
            var danhgia = new Danhgia()
            {
                Idkh = danhGiaDTO.Idkh,
                Noidungdanhgia = danhGiaDTO.Noidungdanhgia,
                Ngaydanhgia = danhGiaDTO.Ngaydanhgia,
                Idhdct = danhGiaDTO.Idhdct,
                Sosao = danhGiaDTO.Sosao,
            };

            await _repos.Create(danhgia);
            await _repos.SaveChanges();
            danhGiaDTO.Id = danhgia.Id;
        }

        public async Task Delete(int id)
        {
            await _repos.Delete(id);
            await _repos.SaveChanges();
        }

        public override bool Equals(object? obj)
        {
            return obj is DanhGiaService service &&
                   EqualityComparer<IDanhGiaRepo>.Default.Equals(_repos, service._repos) &&
                   EqualityComparer<IHinhAnhRepo>.Default.Equals(_hinhAnhRepo, service._hinhAnhRepo);
        }

        public async Task<List<DanhgiaDTO>> GetAll()
        {
            var list = await _repos.GetAll();
            var anh = await _hinhAnhRepo.GetAllAsync();
            return list.Select(list => new DanhgiaDTO()
            {
                Id = list.Id,
                Idkh = list.Idkh,
                Noidungdanhgia = list.Noidungdanhgia,
                Ngaydanhgia = list.Ngaydanhgia,
                Idhdct = list.Idhdct,
                Sosao = list.Sosao,
            }).ToList();

        }

        public async Task<DanhgiaDTO> GetById(int id)
        {
            var list = await _repos.GetById(id);
            return new DanhgiaDTO()
            {
                Id = list.Id,
                Idkh = list.Idkh,
                Noidungdanhgia = list.Noidungdanhgia,
                Ngaydanhgia = list.Ngaydanhgia,
                Idhdct = list.Idhdct,
                Sosao = list.Sosao,
            };
        }

        public async Task<DanhgiaDTO> getByidHDCT(int id)
        {
            var list = await _repos.getByidHDCT(id);
            if (list == null)
            {
                return null;
            }
            var anh = await _hinhAnhRepo.GetByIdDanhGiaAsync(list.Id);

            return new DanhgiaDTO()
            {
                Id = list.Id,
                Idkh = list.Idkh,
                Noidungdanhgia = list.Noidungdanhgia,
                Ngaydanhgia = list.Ngaydanhgia,
                Idhdct = list.Idhdct,
                Sosao = list.Sosao  ,
                Hinhanhs = anh.Select(item => new HinhanhDTO()
                {
                    Id = item.Id,
                    Idtrahang = item.Idtrahang,
                    Iddanhgia = item.Iddanhgia,
                    Urlhinhanh = item.Urlhinhanh,
                }).ToList()

            };

        }

        public async Task<List<DanhgiaDTO>> GetByidSP(int idsp)
        {
            var list = await _repos.GetByidSP(idsp);
            if (list == null || !list.Any()) // Kiểm tra nếu list null hoặc rỗng
            {
                return null;
            }

            // Ánh xạ từng phần tử trong list thành DanhGiaDTO
            return list.Select(item => new DanhgiaDTO()
            {
                Id = item.Id,
                Idkh = item.Idkh,
                Noidungdanhgia = item.Noidungdanhgia,
                Ngaydanhgia = item.Ngaydanhgia,
                Idhdct = item.Idhdct,
                Sosao = item.Sosao,
            }).ToList();
        }

        public async Task Update(int id, DanhgiaDTO danhGiaDTO)
        {
            var itemUpdate = await _repos.GetById(id);


            itemUpdate.Idhdct = danhGiaDTO.Idhdct;
            itemUpdate.Ngaydanhgia = danhGiaDTO.Ngaydanhgia;
            itemUpdate.Idkh = danhGiaDTO.Idkh;
            itemUpdate.Noidungdanhgia = danhGiaDTO.Noidungdanhgia;
            itemUpdate.Sosao = danhGiaDTO.Sosao;

            await _repos.Update(itemUpdate);
            await _repos.SaveChanges();
            danhGiaDTO.Id = itemUpdate.Id;
        }
    }
}
