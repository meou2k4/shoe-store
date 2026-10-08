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
    public class DiaChiService : IDiaChiService
    {
        private readonly IDiaChiRepo diaChiRepos;

        public DiaChiService(IDiaChiRepo diaChiRepos)
        {
            this.diaChiRepos = diaChiRepos;
        }

        public async Task Create(DiachiDTO diachi)
        {
            var Diachi = new Diachi()
            {
                Thanhpho = diachi.Thanhpho,
                Idkh = diachi.Idkh,
                Diachicuthe = diachi.Diachicuthe,
                Quanhuyen = diachi.Quanhuyen,
                Phuongxa = diachi.Phuongxa,
                Tennguoinhan = diachi.Tennguoinhan,
                sdtnguoinhan = diachi.sdtnguoinhan,
                trangthai = diachi.trangthai,

            };
            await diaChiRepos.Create(Diachi);
            await diaChiRepos.SaveChanges();
        }

        public async Task Delete(int id)
        {
            await diaChiRepos.Delete(id);
            await diaChiRepos.SaveChanges();
        }

        public async Task<IEnumerable<DiachiDTO>> GetAllDiaChi()
        {
            var diaChis = await diaChiRepos.GetAllDiaChi();
            return diaChis.Select(diaChis => new DiachiDTO()
            {
                Id = diaChis.Id,
                Diachicuthe = diaChis.Diachicuthe,
                Thanhpho = diaChis.Thanhpho,
                Phuongxa = diaChis.Phuongxa,
                Quanhuyen = diaChis.Quanhuyen,
                Idkh = diaChis.Idkh,
                Tennguoinhan = diaChis.Tennguoinhan,
                sdtnguoinhan = diaChis.sdtnguoinhan,
                trangthai = diaChis.trangthai,
            });
        }

        public async Task<Diachi> GetByIdAsync(int id)
        {

            var diaChi = await diaChiRepos.GetByIdAsync(id);
            if (diaChi == null) throw new KeyNotFoundException("Không tìm thấy Dịa chỉ");
            return new Diachi()
            {
                Id = diaChi.Id,
                Diachicuthe = diaChi.Diachicuthe,
                Thanhpho = diaChi.Thanhpho,
                Phuongxa = diaChi.Phuongxa,
                Quanhuyen = diaChi.Quanhuyen,
                Idkh = diaChi.Idkh,
                Tennguoinhan = diaChi.Tennguoinhan,
                sdtnguoinhan = diaChi.sdtnguoinhan,
                trangthai = diaChi.trangthai,
            };
        }

        public async Task<List<DiachiDTO>> GetDiaChiByIdKH(int idspct)
        {
            try
            {
                var results = await diaChiRepos.GetDiaChiByIdKH(idspct);

                var dtoList = results.Select(result => new DiachiDTO
                {
                    Id = result.Id,
                    Idkh = result.Idkh,
                    Thanhpho = result.Thanhpho,
                    Quanhuyen = result.Quanhuyen,
                    Phuongxa = result.Phuongxa,
                    Diachicuthe = result.Diachicuthe,
                    Tennguoinhan = result.Tennguoinhan,
                    sdtnguoinhan = result.sdtnguoinhan,
                    trangthai = result.trangthai,
                }).ToList();

                return dtoList;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm địa chỉ khách hàng: " + ex.Message);
            }
        }

        public async Task Update(int id, DiachiDTO DiachiDTO)
        {
            var diaChi = await diaChiRepos.GetByIdAsync(id);
            if (diaChi == null) throw new KeyNotFoundException("Không tìm thấy Địa chỉ");

            diaChi.Quanhuyen = DiachiDTO.Quanhuyen;
            diaChi.Thanhpho = DiachiDTO.Thanhpho;
            diaChi.Diachicuthe = DiachiDTO.Diachicuthe;
            diaChi.Idkh = DiachiDTO.Idkh;
            diaChi.Phuongxa = DiachiDTO.Phuongxa;
            diaChi.Tennguoinhan = DiachiDTO.Tennguoinhan;
            diaChi.sdtnguoinhan = DiachiDTO.sdtnguoinhan;
            diaChi.trangthai = DiachiDTO.trangthai;

            await diaChiRepos.Update(diaChi); 
        }
    }
}
