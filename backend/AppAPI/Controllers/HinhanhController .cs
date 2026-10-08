using AppData.DTO;
using AppData.IService;
using AppData.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HinhanhController : ControllerBase
    {
        private readonly IHinhAnhService _Service;

        public HinhanhController(IHinhAnhService service)
        {
            _Service = service;
        }

        // API để lấy tất cả Hình ảnh trả hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hoadonList = await _Service.GetAllAsync();
            return Ok(hoadonList);
        }

        // API để lấy Hình ảnh trả hàng theo Id
        [HttpGet("TraHang/{id}")]
        public async Task<IActionResult> GetByIdTraHang(int id)
        {
            var hinhanhs = await _Service.GetByIdTraHangAsync(id);

            if (hinhanhs == null || !hinhanhs.Any())
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh của đánh giá" });
            }

            var imageUrls = new List<object>();

            foreach (var hinhanh in hinhanhs)
            {
                if (string.IsNullOrEmpty(hinhanh.Urlhinhanh))
                {
                    continue; // Bỏ qua nếu không có đường dẫn ảnh
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture", hinhanh.Urlhinhanh);

                if (!System.IO.File.Exists(filePath))
                {
                    continue; // Bỏ qua nếu file không tồn tại
                }

                var imageUrl = $"{Request.Scheme}://{Request.Host}/picture/{hinhanh.Urlhinhanh}";

                imageUrls.Add(new
                {
                    Url = imageUrl
                });
            }

            if (!imageUrls.Any())
            {
                return NotFound(new { message = "Không có hình ảnh hợp lệ" });
            }

            return Ok(imageUrls);
        }

        [HttpGet("DanhGia/{id}")]
        public async Task<IActionResult> GetByIdDanhGia(int id)
        {
            var hinhanhs = await _Service.GetByIdDanhGiaAsync(id);

            if (hinhanhs == null || !hinhanhs.Any())
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh của đánh giá" });
            }

            var imageUrls = new List<object>();

            foreach (var hinhanh in hinhanhs)
            {
                if (string.IsNullOrEmpty(hinhanh.Urlhinhanh))
                {
                    continue; // Bỏ qua nếu không có đường dẫn ảnh
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture", hinhanh.Urlhinhanh);

                if (!System.IO.File.Exists(filePath))
                {
                    continue; // Bỏ qua nếu file không tồn tại
                }

                var imageUrl = $"{Request.Scheme}://{Request.Host}/picture/{hinhanh.Urlhinhanh}";

                imageUrls.Add(new
                {
                    id = hinhanh.Id,
                    Url = imageUrl
                });
            }

            if (!imageUrls.Any())
            {
                return NotFound(new { message = "Không có hình ảnh hợp lệ" });
            }

            return Ok(imageUrls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hoadon = await _Service.GetByIdAsync(id);
            if (hoadon == null) return NotFound(new { message = "Hình ảnh tìm theo mã không tìm thấy" });
            return Ok(hoadon);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] HinhanhDTO dto, IFormFile? file)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }
            try
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
                    dto.Urlhinhanh = fileName;
                }
                // Thêm hình ảnh (hoặc Hình ảnh trả hàng tùy theo context)
                await _Service.AddAsync(dto);

                // Trả về ID của hình ảnh vừa được thêm
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi thêm hình ảnh
                return StatusCode(500, new { message = "Lỗi khi thêm hình ảnh", error = ex.Message });
            }
        }


        // API để cập nhật Hình ảnh trả hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HinhanhDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            var existingHoadon = await _Service.GetByIdAsync(id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hình ảnh trả hàng không tìm thấy" });
            }

            try
            {
                await _Service.UpdateAsync(dto, id);
                return NoContent(); // Trả về status code 204 nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi cập nhật Hình ảnh trả hàng
                return StatusCode(500, new { message = "Lỗi khi cập nhật Hình ảnh trả hàng", error = ex.Message });
            }
        }

        // API để xóa Hình ảnh trả hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingHoadon = await _Service.GetByIdAsync(id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hình ảnh trả hàng không tìm thấy" });
            }

            try
            {
                await _Service.DeleteAsync(id);
                return NoContent(); // Trả về status code 204 nếu xóa thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi xóa Hình ảnh trả hàng
                return StatusCode(500, new { message = "Lỗi khi xóa Hình ảnh trả hàng", error = ex.Message });
            }
        }
    }
}
