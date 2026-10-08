using AppData.DTO;
using AppData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.IService
{
    public interface IColorService
    {
        Task<IEnumerable<ColorDTO>> GetAllAsync();
        Task<ColorDTO> GetByIdAsync(int id);
        Task<ColorDTO> AddAsync(ColorDTO dto);
        Task<ColorDTO> UpdateAsync(int id, ColorDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<List<int>> GetColorsForProductAsync(int productId);
         bool IsColorUsedInProduct(int colorId);
        Task<List<Color>> GetListByIdsAsync();

    }
}
