using AppData.IRepository;
using AppData.Models;
using AppData.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Repository
{
    public class SizeRepo : ISizeRepo
    {
        private readonly AppDbContext _context;
        public SizeRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Models.Size>> GetAllAsync()
        {
            return await _context.Set<Models.Size>().ToListAsync();
        }

        public async Task<Models.Size> GetByIdAsync(int id)
        {
            return await _context.Set<Models.Size>().FindAsync(id);
        }

        public async Task<Models.Size> AddAsync(Models.Size entity)
        {
            _context.Set<Models.Size>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Models.Size> UpdateAsync(Models.Size entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<Models.Size>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Models.Size>> GetListByIdsAsync()
        {
            var sizes = _context.sizes
            .ToListAsync();

            return sizes;
        }
    }
}
