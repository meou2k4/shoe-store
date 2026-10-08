using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppData.IRepository;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppData.Repository
{
	public class KhachhangRepostory : Ikhachhangrepository
	{
		private readonly AppDbContext _context;
		public KhachhangRepostory(AppDbContext context)
		{
			_context = context;

		}
		public async Task<Khachhang> GetByIdAsyncThao(int id)
		{
			return await _context.khachhangs.FindAsync(id);
		}

		public async Task<IEnumerable<Khachhang>> GetAllAsyncThao()
		{
			return await _context.khachhangs.Where(x=> x.Trangthai==0 || x.Trangthai ==1).ToListAsync();
		}

		public async Task<Khachhang> CreateAsyncThao(Khachhang entity)
		{
			if (await _context.khachhangs.AnyAsync(x => x.Sdt == entity.Sdt && (x.Trangthai == 0 || x.Trangthai ==1)))
				throw new Exception("Số điện thoại đã được sử dụng.");

			if (await _context.khachhangs.AnyAsync(x => x.Email == entity.Email && (x.Trangthai == 0 || x.Trangthai ==1)))
				throw new Exception("Email đã được sử dụng.");

			await _context.khachhangs.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<Khachhang> UpdateAsyncThao(Khachhang entity)
		{
			var existing = await _context.khachhangs.FindAsync(entity.Id);
			if (existing == null) throw new Exception("Khách hàng không tồn tại.");

			existing.Ten = entity.Ten;
			existing.Sdt = entity.Sdt;
			existing.Ngaysinh = entity.Ngaysinh;
			existing.Tichdiem = entity.Tichdiem;
			existing.Email = entity.Email;
			existing.Ngaytaotaikhoan = entity.Ngaytaotaikhoan;
			existing.Diemsudung = entity.Diemsudung;
			existing.Trangthai = entity.Trangthai;
			existing.Idrank = entity.Idrank;

			await _context.SaveChangesAsync();
			return existing;
		}

		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await _context.Database.BeginTransactionAsync();
		}
		public async Task DeleteAsyncThao(Khachhang khachhang)
		{
			_context.khachhangs.Remove(khachhang);
			await _context.SaveChangesAsync();
		}
		public async Task<bool> CheckForeignKeyConstraintAsync(int khachhangId)
		{
			return await _context.hoadons
				.AnyAsync(h => h.Idkh == khachhangId);  
		}
		public async Task DeleteDanhgiasByKhachhangIdAsync(int khachhangId)
		{
			var danhgias = await _context.danhgias.Where(d => d.Idkh == khachhangId).ToListAsync();
			if (danhgias.Any())
			{
				_context.danhgias.RemoveRange(danhgias);
				await _context.SaveChangesAsync();
			}
		}
		public async Task UpdateAsync(Giamgia giamgia)
		{
			_context.giamgias.Update(giamgia);
			await _context.SaveChangesAsync();
		}
		public async Task<IEnumerable<Khachhang>> SearchByNameAsyncThao(string name)
		{
			return await _context.khachhangs
				.Where(kh => kh.Ten.Contains(name) && (kh.Trangthai == 0||kh.Trangthai==1))
				.ToListAsync();
		}

		public async Task<IEnumerable<Khachhang>> SearchBySdtAsyncThao(string sdt)
		{
			return await _context.khachhangs
				.Where(kh => kh.Sdt.Contains(sdt))
				.ToListAsync();
		}
		

		public async Task<IEnumerable<Khachhang>> SearchByEmailAsyncThao(string email)
		{
			return await _context.khachhangs
				.Where(kh => kh.Email.Contains(email))
				.ToListAsync();
		}public async Task<IEnumerable<Khachhang>> SearchByEmailTenSdtAsync(string keyword)
		{
			return await _context.khachhangs
				.Where(kh => (kh.Email.Contains(keyword) || kh.Sdt.Contains(keyword)|| kh.Ten.Contains(keyword))&& kh.Trangthai==0)
				.ToListAsync();
		}

	}
	}
