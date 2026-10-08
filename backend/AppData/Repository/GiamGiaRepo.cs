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
    public class GiamGiaRepo : IGiamGiaRepo
    {
        private readonly AppDbContext _context;
        public GiamGiaRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Giamgia> GetByIdAsync(int id)
        {
            var giamgia = await _context.giamgias
                .OrderBy(g => g.Donvi != 1)
                .ThenBy(g => g.Giatri)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (giamgia != null)
            {
                giamgia.Trangthai = GetTrangThai(giamgia);
                await _context.SaveChangesAsync();
            }

            return giamgia;
        }

        private int GetTrangThai(Giamgia g)
        {
            var now = DateTime.Now;

            if (g.Soluong == 0)
                return 2;
            else if (g.Ngaybatdau > now)
                return 1;
            else if (g.Ngayketthuc < now)
                return 2;

            return g.Trangthai; // giữ nguyên nếu không rơi vào các điều kiện trên
        }


        public async Task<IEnumerable<Giamgia>> GetAllAsync()
        {
            var giamgias = await _context.giamgias
                .OrderBy(g => g.Donvi != 1)
                .ThenBy(g => g.Giatri)
                .ToListAsync();

            foreach (var g in giamgias)
            {
                g.Trangthai = GetTrangThai(g);
            }

            // Nếu muốn cập nhật trực tiếp vào DB (nếu có thay đổi):
            await _context.SaveChangesAsync();

            return giamgias;
        }


        public async Task<Giamgia> AddAsync(Giamgia giamgia)
        {
            _context.giamgias.Add(giamgia);
            await _context.SaveChangesAsync();
            return giamgia;
        }

        public async Task<Giamgia> UpdateAsync(Giamgia giamgia)
        {
            _context.giamgias.Update(giamgia);
            await _context.SaveChangesAsync();
            return giamgia;
        }

        public async Task DeleteAsync(Giamgia giamgia)
        {
            giamgia.Trangthai = 4;
            _context.giamgias.Update(giamgia);
            await _context.SaveChangesAsync();
        }

        public async Task AddRankToGiamgia(int giamgiaId, List<string> rankNames)
        {
            var giamgia = await _context.giamgias.FindAsync(giamgiaId);
            if (giamgia == null) throw new Exception("Giảm giá không tồn tại");

            foreach (var rankName in rankNames)
            {
                var rank = await _context.ranks.FirstOrDefaultAsync(r => r.Tenrank == rankName);
                if (rank != null)
                {
                    var giamgiaRank = new giamgia_rank { IDgiamgia = giamgiaId, Idrank = rank.Id };
                    _context.giamgia_Ranks.Add(giamgiaRank);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
