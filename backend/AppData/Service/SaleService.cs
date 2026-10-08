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
    public class SaleService : ISaleService
    {
        private readonly ISaleRepo _repository;
        public SaleService(ISaleRepo repository)
        {
            _repository = repository;

        }
        /*public async Task<IEnumerable<SaleDto>> GetAllAsync()
		{
			var sales = await _repository.GetAllAsync();
			return sales.Select(s => new SaleDto
			{
				Ten = s.Ten,
				Mota = s.Mota,
				Trangthai = s.Trangthai,
				Ngaybatdau = s.Ngaybatdau,
				Ngayketthuc = s.Ngayketthuc
			});
		}*/
        public async Task<IEnumerable<Sale>> GetAllWithIdAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<SaleDTO?> GetByIdAsync(int id)
        {
            var sale = await _repository.GetByIdAsync(id);
            if (sale == null) return null;

            return new SaleDTO
            {
                Ten = sale.Ten,
                Mota = sale.Mota,
                Trangthai = sale.Trangthai,
                Ngaybatdau = sale.Ngaybatdau,
                Ngayketthuc = sale.Ngayketthuc
            };
        }

        public async Task AddAsync(SaleDTO saleDto)
        {



            // Kiểm tra ngày kết thúc
            if (saleDto.Ngayketthuc < saleDto.Ngaybatdau)
            {
                throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            // Tạo một đối tượng Sale mới
            Sale newSale = new Sale
            {
                Ten = saleDto.Ten,
                Mota = saleDto.Mota,
                Ngaybatdau = saleDto.Ngaybatdau,
                Ngayketthuc = saleDto.Ngayketthuc,
            };

            // Xác định trạng thái
            if (saleDto.Ngaybatdau <= DateTime.Now && saleDto.Ngayketthuc >= DateTime.Now)
            {
                newSale.Trangthai = 0; // Đang diễn ra
            }
            else if (saleDto.Ngaybatdau > DateTime.Now)
            {
                newSale.Trangthai = 1; // Chuẩn bị diễn ra
            }
            else /*if (saleDto.Ngayketthuc < DateTime.Now)*/
            {
                newSale.Trangthai = 2; // Đã diễn ra
            }

            await _repository.AddAsync(newSale);

        }
        public async Task UpdateStatusLoad(int id)
        {

            var sale = await _repository.GetByIdAsync(id);
            if (sale == null)
            {
                throw new KeyNotFoundException("Sale không tồn tại");
            }
            if (sale.Trangthai != 3)
            {
                // Cập nhật trạng thái dựa trên ngày bắt đầu và ngày kết thúc
                if (sale.Ngaybatdau <= DateTime.Now && sale.Ngayketthuc >= DateTime.Now)
                {
                    sale.Trangthai = 0; // Đang diễn ra
                }
                else if (sale.Ngaybatdau > DateTime.Now)
                {
                    sale.Trangthai = 1; // Chuẩn bị diễn ra
                }
                else if (sale.Ngayketthuc < DateTime.Now)
                {
                    sale.Trangthai = 2; // Đã diễn ra
                }
            }


            await _repository.UpdateAsync(sale);
        }
        public async Task UpdateStatusToCancelled(int id)
        {
            var sale = await _repository.GetByIdAsync(id);
            if (sale == null)
            {
                throw new KeyNotFoundException("Sale không tồn tại");
            }

            sale.Trangthai = 3; // Cập nhật trạng thái thành Hủy
            await _repository.UpdateAsync(sale);
        }

        public async Task UpdateStatusBasedOnDates(int id)
        {
            var sale = await _repository.GetByIdAsync(id);
            if (sale == null)
            {
                throw new KeyNotFoundException("Sale không tồn tại");
            }

            // Cập nhật trạng thái dựa trên ngày bắt đầu và ngày kết thúc
            if (sale.Ngaybatdau <= DateTime.Now && sale.Ngayketthuc >= DateTime.Now)
            {
                sale.Trangthai = 0; // Đang diễn ra
            }
            else if (sale.Ngaybatdau > DateTime.Now)
            {
                sale.Trangthai = 1; // Chuẩn bị diễn ra
            }
            else if (sale.Ngayketthuc < DateTime.Now)
            {
                sale.Trangthai = 2; // Đã diễn ra
            }

            await _repository.UpdateAsync(sale);
        }
        public async Task UpdateAsync(int id, SaleDTO saleDto)
        {
            // Tìm kiếm Sale theo ID
            var sale = await _repository.GetByIdAsync(id);

            if (sale == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy chương trình Sale với ID = {id}.");
            }

            // Cập nhật các thông tin khác (trừ trang thái)
            sale.Ten = saleDto.Ten ?? sale.Ten; // Giữ nguyên nếu giá trị mới là null
            sale.Mota = saleDto.Mota ?? sale.Mota;
            sale.Ngaybatdau = saleDto.Ngaybatdau;
            sale.Ngayketthuc = saleDto.Ngayketthuc;

            /*// Cập nhật nếu có sự thay đổi ở trạng thái
			if (saleDto.Trangthai.HasValue && sale.Trangthai != saleDto.Trangthai)
			{
				sale.Trangthai = saleDto.Trangthai.Value;
			}*/

            // Gọi hàm cập nhật từ repository
            await _repository.UpdateAsync(sale);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
