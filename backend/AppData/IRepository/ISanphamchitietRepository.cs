using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IRepository
{
	public interface ISanphamchitietRepository
	{
		Task<IEnumerable<Giohangchitiet>> GetGiohangchitietBySpctIdAsync(int spctId);
		Task<IEnumerable<Salechitiet>> GetSalechitietBySpctIdAsync(int spctId);
		Task<IEnumerable<Hoadonchitiet>> GetHoadonchitietBySpctIdAsync(int spctId);
		Task SaveChangesAsync();
		void DeleteGiohangchitiet(Giohangchitiet giohangchitiet);
		void DeleteSalechitiet(Salechitiet salechitiet);
		void DeleteSanphamchitiet(Sanphamchitiet sanphamchitiet);
		Task<IEnumerable<Sanphamchitiet>> GetAllAsync();
		Task<Sanphamchitiet> GetByIdAsync(int id);
		Task AddAsync(Sanphamchitiet entity);
		Task UpdateAsync(Sanphamchitiet entity);
		Task DeleteAsync(int id);
		void Update(Sanphamchitiet sanphamchitiet);
	}
}
