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
	public interface IsanphamRepos
	{
		Task<IEnumerable<SanphamDetailDto>> GetSanphamDetailsAsync();
		Task<List<ProductWithAttributesDTO>> GetAllActiveProductsWithAttributesAsync(int id);
		Task<IEnumerable<Sanpham>> GetAllAsync();
		Task<Sanpham> GetByIdAsync(int id);
		Task AddAsync(Sanpham sanpham);
		Task UpdateAsync(Sanpham sanpham);
		Task AddSoluongAsync(int id, int soluongThem);
		Task DeleteAsync(int id);
		Task<IEnumerable<Sanpham>> SearchByNameAsync(string name);
		Task<IEnumerable<Sanpham>> SearchByNameHdAsync(string name);
		Task<int> CountTrangThai0or1Async();
	}
}
