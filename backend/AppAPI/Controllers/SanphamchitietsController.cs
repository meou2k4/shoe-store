using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppData;
using AppData.Models;
using AppData.DTO;
using AppData.IService;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanphamchitietsController : ControllerBase
    {
        private readonly ISanPhamChiTietService _service;
        public SanphamchitietsController(ISanPhamChiTietService service)
        {
            _service = service;

        }
        // API để lấy tất cả sản phẩm chi tiết
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var SanPhamCTList = await _service.GetAllAsync();
            return Ok(SanPhamCTList);
        }

        // API để lấy sản phẩm chi tiết theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var SanPhamCT = await _service.GetByIdAsync(id);
            if (SanPhamCT == null) return NotFound(new { message = "Sản phẩm chi tiết không tìm thấy" });
            return Ok(SanPhamCT);
        }

        [HttpGet("GetImageById/{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var SanPhamCT = await _service.GetByIdAsync(id);
            if (SanPhamCT == null || string.IsNullOrEmpty(SanPhamCT.UrlHinhanh))
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh của sản phẩm chi tiết" });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture", SanPhamCT.UrlHinhanh);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Tệp hình ảnh không tồn tại" });
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var mimeType = "image/jpeg"; // Giả định file là JPEG, có thể kiểm tra kiểu tệp thực tế

            return File(fileStream, mimeType);
        }


        [HttpGet("sanpham/{id}")]
        public async Task<IActionResult> GetByIdSPAsync(int id)
        {
            try
            {
                var thuoctinhDto = await _service.GetByIdSPAsync(id);

                if (thuoctinhDto == null || !thuoctinhDto.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy sản phẩm trong sản phẩm chi tiết với ID: " + id });
                }

                return Ok(thuoctinhDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SanphamchitietDTO dto, IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Chỉ lưu tên file vào UrlHinhanh
                dto.UrlHinhanh = fileName;
            }

            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] SanphamchitietDTO dto, IFormFile? file)
        {
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Xóa ảnh cũ nếu có ảnh mới
            if (file != null && file.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture");

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingItem.UrlHinhanh))
                {
                    var oldFilePath = Path.Combine(folderPath, existingItem.UrlHinhanh);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh mới
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                dto.UrlHinhanh = fileName; // Cập nhật tên file mới
            }
            else
            {
                // Giữ nguyên ảnh cũ nếu không có ảnh mới
                dto.UrlHinhanh = existingItem.UrlHinhanh;
            }

            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingItem = await _service.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Xóa ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(existingItem.UrlHinhanh))
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture");
                var filePath = Path.Combine(folderPath, existingItem.UrlHinhanh);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }

    }
}
