using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Service
{
    public class SaleChiTietService : ISaleChiTietService
    {
        private readonly ISaleChiTietRepo _repository;
        private readonly ISanPhamChiTietRepo _SPCTrepository;
        private readonly ISaleRepo _Salerepository;
        public SaleChiTietService(ISaleChiTietRepo repository, ISanPhamChiTietRepo SPCTrepository, ISaleRepo Salerepository)
        {
            _repository = repository;
            _SPCTrepository = SPCTrepository;
            _Salerepository = Salerepository;
        }

        public async Task<IEnumerable<Salechitiet>> GetAllAsync()
        {

            var entities = await _repository.GetAllAsync();

            return entities.Select(salect => new Salechitiet
            {
                Id = salect.Id,
                Idsale = salect.Idsale,
                Idspct = salect.Idspct,
                Donvi = salect.Donvi,
                Soluong = salect.Soluong,
                Giatrigiam = salect.Giatrigiam
            });
        }

        public async Task<Salechitiet> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new Salechitiet
            {
                Id = entity.Id,
                Idsale = entity.Idsale,
                Idspct = entity.Idspct,
                Donvi = entity.Donvi,
                Soluong = entity.Soluong,
                Giatrigiam = entity.Giatrigiam
            };
        }

        public async Task<Salechitiet> GetByIdAsyncSpct(int id)
        {
            // Lấy danh sách Salechitiet từ repository
            var salechitiets = await _repository.GetByIdAsyncSpct(id);

            // Kiểm tra nếu không có dữ liệu trả về
            if (salechitiets == null || !salechitiets.Any())
                return null;

            // Lọc các sale đang hoạt động trong khoảng thời gian
            var activeSales = salechitiets
                              .Where(sale =>
                                  sale.Sale.Trangthai == 0 && // Sale đang hoạt động
                                  sale.Sale.Ngaybatdau <= DateTime.Now && // Ngày bắt đầu <= hiện tại
                                  sale.Sale.Ngayketthuc >= DateTime.Now // Ngày kết thúc >= hiện tại
                              );

            // Ưu tiên lấy sale có Donvi == 0, sau đó chọn Giatrigiam lớn nhất
            var prioritizedSale = activeSales
                                  .OrderByDescending(sale => sale.Donvi == 1) // Đưa sale có Donvi == 0 lên đầu
                                  .ThenByDescending(sale => sale.Giatrigiam)  // Ưu tiên Giatrigiam lớn nhất
                                  .FirstOrDefault(); // Lấy bản ghi đầu tiên

            // Nếu không tìm thấy sale thỏa mãn, trả về null
            if (prioritizedSale == null) return null;

            // Trả về đối tượng Salechitiet đã được tạo từ thông tin prioritizedSale
            return new Salechitiet
            {
                Id = prioritizedSale.Id,
                Idsale = prioritizedSale.Idsale,
                Idspct = prioritizedSale.Idspct,
                Donvi = prioritizedSale.Donvi,
                Soluong = prioritizedSale.Soluong,
                Giatrigiam = prioritizedSale.Giatrigiam
            };
        }

        public async Task AddAsync(SalechitietDTO spctDTO)
        {
            var idspct = await _SPCTrepository.GetByIdAsync(spctDTO.Idspct);
            if (idspct == null) throw new ArgumentNullException("Sản phẩm chi tiết không tồn tại");

            var entity = await _Salerepository.GetByIdAsync(spctDTO.Idsale);
            if (entity == null) throw new ArgumentNullException("Sale không tồn tại");


            // Tạo đối tượng Hoadon từ DTO
            var salect = new Salechitiet
            {
                Idsale = spctDTO.Idsale,
                Idspct = spctDTO.Idspct,
                Donvi = spctDTO.Donvi,
                Soluong = spctDTO.Soluong,
                Giatrigiam = spctDTO.Giatrigiam
            };

            // Thêm hóa đơn vào cơ sở dữ liệu
            await _repository.AddAsync(salect);
        }


        // Phương thức cập nhật hoá đơn
        public async Task UpdateAsync(SalechitietDTO dto, int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Hóa đơn không tồn tại");

            var idspct = await _SPCTrepository.GetByIdAsync(dto.Idspct);
            if (idspct == null) throw new ArgumentNullException("Sản phẩm chi tiết không tồn tại");

            var sale = await _Salerepository.GetByIdAsync(dto.Idsale);
            if (sale == null) throw new ArgumentNullException("Sale không tồn tại");


            if (entity != null)
            {
                entity.Idsale = dto.Idsale;
                entity.Idspct = dto.Idspct;
                entity.Donvi = dto.Donvi;
                entity.Soluong = dto.Soluong;
                entity.Giatrigiam = dto.Giatrigiam;

                await _repository.UpdateAsync(entity);
            }
        }

        // Phương thức xóa hoá đơn
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
