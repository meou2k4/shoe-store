using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppData.IRepository
{
	public interface Ikhachhangrepository
	{
		Task<Khachhang> GetByIdAsyncThao(int id);
		Task<IEnumerable<Khachhang>> GetAllAsyncThao();
		Task<Khachhang> CreateAsyncThao(Khachhang entity);
		Task<Khachhang> UpdateAsyncThao(Khachhang entity);
		Task DeleteAsyncThao(Khachhang khachhang);
		Task<bool> CheckForeignKeyConstraintAsync(int khachhangId);
		Task<IEnumerable<Khachhang>> SearchByNameAsyncThao(string name);
		Task<IEnumerable<Khachhang>> SearchBySdtAsyncThao(string sdt);
		Task<IEnumerable<Khachhang>> SearchByEmailAsyncThao(string email);
		Task<IEnumerable<Khachhang>> SearchByEmailTenSdtAsync(string keyword);

		Task<IDbContextTransaction> BeginTransactionAsync();
		Task DeleteDanhgiasByKhachhangIdAsync(int khachhangId);


	}
}
