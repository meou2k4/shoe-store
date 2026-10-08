using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.EntityFrameworkCore;

namespace AppData.Repository
{
	public class SaleechitietRepos : IsalechitietRepos
	{
		private readonly AppDbContext _context;
        public SaleechitietRepos(AppDbContext context)
		{
			_context = context;

		}

		public async Task<Salechitiet> GetByIdAsync(int id)
		{
			return await _context.salechitiets.Include(x => x.spchitiet).Include(x => x.Sale)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<Salechitiet>> GetAllAsync()
		{
			return await _context.salechitiets.Include(x => x.spchitiet).Include(x => x.Sale).ToListAsync();
		}

		public async Task CreateAsync(Salechitiet salechitiet)
		{
			await _context.salechitiets.AddAsync(salechitiet);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Salechitiet salechitiet)
		{
			_context.salechitiets.Update(salechitiet);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var salechitiet = await GetByIdAsync(id);
			if (salechitiet != null)
			{
				_context.salechitiets.Remove(salechitiet);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<Salechitiet> GetBySaleIdSanphamchitietAsync(int saleid, int spctid)
		{ var x = await _context.salechitiets.Where(x => x.Idsale == saleid && x.Idspct == 19).FirstAsync();
			if(x != null)
			{
				return x;
			}
			else
			{
				return null;
			}
		}
	}
}
