using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class ColorRepo : IColorRepo
    {
        private readonly AppDbContext _context;
        public ColorRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Models.Color>> GetAllAsync()
        {
            return await _context.Set<Models.Color>().ToListAsync();
        }

        public async Task<Models.Color> GetByIdAsync(int id)
        {
            return await _context.Set<Models.Color>().FindAsync(id);
        }
        public async Task<List<int>> GetUniqueColorsByProductIdAsync(int productId)
        {
            var uniqueColors = await _context.colors
                .Where(spm => _context.Sanphamchitiets
                    .Where(spct => spct.Idsp == productId)
                    .Select(spct => spct.IdMau)
                    .Contains(spm.Id))
                .Select(spm => spm.Id)
                .Distinct()
                .ToListAsync();

            return uniqueColors;
        }


        public async Task<Models.Color> AddAsync(Models.Color entity)
        {
            _context.Set<Models.Color>().Add(entity);
            await _context.SaveChangesAsync();

            // Truy vấn lại dữ liệu từ DB sau khi thêm thành công
            return await _context.Set<Models.Color>()
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Tenmau == entity.Tenmau && x.Mamau == entity.Mamau
                                                        && x.Trangthai == entity.Trangthai);
        }

        public async Task<Models.Color> UpdateAsync(Models.Color entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<Models.Color>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Color>> GetListByIdsAsync()
        {
            var colors = _context.colors
            .ToListAsync();

            return colors;
        }
    }
}
