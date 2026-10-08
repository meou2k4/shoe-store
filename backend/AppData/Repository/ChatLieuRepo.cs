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
    public class ChatLieuRepo : IChatLieuRepo
    {
        private readonly AppDbContext _context;
        public ChatLieuRepo(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Models.ChatLieu>> GetAllAsync()
        {
            return await _context.Set<Models.ChatLieu>().ToListAsync();
        }

        public async Task<Models.ChatLieu> GetByIdAsync(int id)
        {
            return await _context.Set<Models.ChatLieu>().FindAsync(id);
        }

        public async Task<Models.ChatLieu> AddAsync(Models.ChatLieu entity)
        {
            _context.Set<Models.ChatLieu>().Add(entity);
            await _context.SaveChangesAsync();

            // Truy vấn lại dữ liệu từ DB sau khi thêm thành công
            return await _context.Set<Models.ChatLieu>()
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Tenchatlieu == entity.Tenchatlieu
                                                        && x.Trangthai == entity.Trangthai);
        }


        public async Task<Models.ChatLieu> UpdateAsync(Models.ChatLieu entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<Models.ChatLieu>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatLieu>> GetListByIdsAsync()
        {
            var chatLieus = await _context.chatLieus
            .ToListAsync();

            return chatLieus;
        }
    }
}
