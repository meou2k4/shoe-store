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
    public class DanhGiaRepo : IDanhGiaRepo
    {
        private readonly AppDbContext _db;

        public DanhGiaRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task Create(Danhgia danhgia)
        {
            await _db.danhgias.AddAsync(danhgia);
        }

        public async Task Delete(int id)
        {
            var item = await GetById(id);
            if (item != null)
            {


                _db.danhgias.Remove(item);
            } 
           
            
        }

        public async Task<List<Danhgia>> GetAll()
        {
            return await _db.danhgias.ToListAsync();
        }

        public async Task<Danhgia> GetById(int id)
        {
            return await _db.danhgias.FindAsync(id);
        }

      
        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }

        public async Task Update(Danhgia danhgia)
        {
            var itemUpdate  = await GetById(danhgia.Id);
            if(itemUpdate != null)
            {
                _db.Entry(danhgia).State = EntityState.Modified;
            }
        }
      
        public async Task<Danhgia> getByidHDCT(int id)
        {
            var resurl = await _db.danhgias.Where(d => d.Idhdct == id).FirstOrDefaultAsync();
            return resurl;

        }

        public async Task<List<Danhgia>> GetByidSP(int idsp)
        {
            // Tìm danh sách đánh giá theo idsp
            var danhgias = await _db.danhgias
                .Include(dg => dg.Khachhang) // Bao gồm thông tin khách hàng
                .Include(dg => dg.Hoadonchitiet) // Bao gồm thông tin hóa đơn chi tiết
                .Where(dg => dg.Hoadonchitiet.Idspchitiet.Sanpham.Id == idsp) // Lọc theo idsp
                .ToListAsync();

            return danhgias;
        }
    }
}
