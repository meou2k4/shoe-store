using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IService_Admin
{
	public interface ISanPhamservice
	{
		Task<List<ProductWithAttributesDTO>> GetAllActiveProductsWithAttributesAsync(int id);
		Task<IEnumerable<Sanpham>> GetAllAsync();
		Task<SanphamDTO> GetByIdAsync(int id);
		Task<bool> AddSoluongAsync(int id, int soluongThem);
		Task AddAsync(SanphamDTO sanphamDto);
	Task<bool> UpdateAsync(int id, SanphamDTO sanphamDto);
		Task DeleteAsync(int id);
		Task UpdateStatusToCancelled(int id);
		Task UpdateStatusLoad(int id);
		Task<IEnumerable<Sanpham>> SearchByNameHdAsync(string name);
	
		Task<IEnumerable<SanphamDTO>> SearchByNameAsync(string name);
		Task<IEnumerable<SanphamDetailDto>> GetSanphamDetailsAsync();
		Task<int> GetTongTrangThai0or1Async();
	}
}
