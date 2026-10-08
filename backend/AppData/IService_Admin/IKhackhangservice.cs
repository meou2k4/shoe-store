using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Dto_Admin;
using AppData.Models;

namespace AppData.IService_Admin
{
	public interface IKhackhangservice
	{
		Task<KhachhangDto> GetByIdAsyncThao(int id);
		Task<IEnumerable<Khachhang>> GetAllAsyncThao();
		Task CreateAsyncThao(KhachhangDto dto);
		Task UpdateAsyncThao(int id, KhachhangDto dto);
		Task<bool> DeleteKhachhangAsyncThao(int Id);
		Task ToggleTrangthaiAsync(int Id);
		Task<IEnumerable<KhachhangDto>> SearchByNameAsyncThao(string name);
		Task<IEnumerable<KhachhangDto>> SearchBySdtAsyncThao(string sdt);
		Task<IEnumerable<KhachhangDto>> SearchByEmailAsyncThao(string email);
		Task<IEnumerable<Khachhang>> SearchByEmailTenSdtAsync(string keyword);


	}
}
