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
    public class SaleRepo : ISaleRepo
    {
        private readonly AppDbContext _context;
        public SaleRepo(AppDbContext context)
        {
            _context = context;

        }
        public async Task AddAsync(Sale sale)
        {
            await _context.sales.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _context.sales.FindAsync(id);
            if (sale != null)
            {
                _context.sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Không tìm thấy chương trình Sale");
            }
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.sales.ToListAsync();
        }

        public async Task<Sale> GetByIdAsync(int id)
        {
            return await _context.sales.FindAsync(id);
        }

        public async Task UpdateAsync(Sale sale)
        {
            _context.sales.Update(sale);
            await _context.SaveChangesAsync();
        }
    }
}
