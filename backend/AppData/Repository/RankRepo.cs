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
    public class RankRepo : IRankRepo
    {
        private readonly AppDbContext _context;
        public RankRepo(AppDbContext context)
        {
            _context = context;

        }
        public async Task AddAsync(Rank rank)
        {
            await _context.Set<Rank>().AddAsync(rank);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rank = await GetByIdAsync(id);
            if (rank != null)
            {
                _context.Set<Rank>().Remove(rank);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tìm thấy NotFoul");
            }
        }

        public async Task<IEnumerable<Rank>> GetAllAsync()
        {
            return await _context.Set<Rank>().ToListAsync();
        }

        public async Task<Rank> GetByIdAsync(int id)
        {
            return await _context.Set<Rank>().FindAsync(id);
        }

        public async Task UpdateAsync(Rank rank)
        {
            _context.Set<Rank>().Update(rank);
            await _context.SaveChangesAsync();
        }
    }
}
