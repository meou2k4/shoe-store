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
    public class ThuongHieuRepo : IThuongHieuRepo
    {
        private readonly AppDbContext _context;
        public ThuongHieuRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Thuonghieu>> GetAllAsync()
        {
            return await _context.Set<Thuonghieu>().ToListAsync();
        }

        public async Task<Thuonghieu> GetByIdAsync(int id)
        {
            return await _context.Set<Thuonghieu>().FindAsync(id);
        }

        public async Task<Thuonghieu> AddAsync(Thuonghieu entity)
        {
            _context.Set<Thuonghieu>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Thuonghieu> UpdateAsync(Thuonghieu entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<Thuonghieu>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
