using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class SaleChiTietRepo : ISaleChiTietRepo
    {
        private readonly AppDbContext _context;

        public SaleChiTietRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Salechitiet>> GetAllAsync()
        {
            try
            {
                return await _context.salechitiets
                                     .Include(h => h.Sale)
                                     .Include(h => h.spchitiet)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sale chi tiết.", ex);
            }
        }

        public async Task<Salechitiet> GetByIdAsync(int id)
        {
            try
            {
                return await _context.salechitiets
                                     .Include(h => h.Sale)
                                     .Include(h => h.spchitiet)
                                     .FirstOrDefaultAsync(h => h.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sale chi tiết với ID {id}.", ex);
            }
        }


        public async Task<List<Salechitiet>> GetByIdAsyncSpct(int id)
        {
            try
            {
                // Lọc danh sách salechitiets dựa trên Idspct và các điều kiện liên quan
                return await _context.salechitiets
                                     .Include(h => h.Sale)         // Bao gồm thông tin liên quan từ bảng Sale
                                     .Include(h => h.spchitiet)    // Bao gồm thông tin liên quan từ bảng spchitiet
                                     .Where(h => h.Idspct == id && h.Soluong > 0) // Lọc theo Idspct và số lượng lớn hơn 0
                                     .ToListAsync();              // Chuyển kết quả thành danh sách
            }
            catch (Exception ex)
            {
                // Ném lỗi có thêm thông tin chi tiết
                throw new Exception($"Lỗi khi lấy danh sách Salechi tiết với Idspct = {id}.", ex);
            }
        }

        public async Task AddAsync(Salechitiet entity)
        {
            try
            {
                await _context.salechitiets.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Lỗi khi thêm sale chi tiết vào cơ sở dữ liệu.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi thêm sale chi tiết.", ex);
            }
        }

        public async Task UpdateAsync(Salechitiet entity)
        {
            try
            {
                _context.salechitiets.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Lỗi khi cập nhật sale chi tiết trong cơ sở dữ liệu.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi cập nhật sale chi tiết.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.salechitiets.FindAsync(id);
                if (entity != null)
                {
                    _context.salechitiets.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Không tìm thấy sale chi tiết với ID {id} để xóa.");
                }
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Lỗi khi xóa sale chi tiết trong cơ sở dữ liệu.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi xóa sale chi tiết.", ex);
            }
        }

    }
}
