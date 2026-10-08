using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Models;

namespace AppData.IRepository
{
	public interface IsalechitietRepos
	{
		Task<Salechitiet> GetByIdAsync(int id);
		Task<Salechitiet> GetBySaleIdSanphamchitietAsync(int saleid,int spctid);
		Task<List<Salechitiet>> GetAllAsync();
		Task CreateAsync(Salechitiet salechitiet);
		Task UpdateAsync(Salechitiet salechitiet);
		Task DeleteAsync(int id);
		
	}
}
