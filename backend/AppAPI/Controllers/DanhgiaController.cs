using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using AppData.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhgiaController : ControllerBase
    {
        private readonly IDanhGiaService _services;
        private readonly IHinhAnhService _hinhAnhService;

        public DanhgiaController(IDanhGiaService services, IHinhAnhService hinhAnhService)
        {
            _services = services;
            _hinhAnhService = hinhAnhService;
        }





        // GET: api/Danhgias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhgiaDTO>>> Getdanhgias()
        {
            try
            {
                if (await _services.GetAll() == null)
                {
                    return NotFound();
                }
                return await _services.GetAll();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Danhgias/5
        [HttpGet("DanhGia/{id}")]
        public async Task<ActionResult<DanhgiaDTO>> GetDanhgia(int id)
        {
            try
            {

              
                var danhgia = await _services.GetById(id);

                if (danhgia == null)
                {
                    return NotFound();
                }

                return danhgia;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/byIDhdct/{id}")]
        public async Task<ActionResult<DanhgiaDTO>> GetDanhgiaByidHDCT(int id)
        {
            try
            {
                var danhgia = await _services.getByidHDCT(id);

                if (danhgia == null)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Không tìm thấy đánh giá cho hóa đơn chi tiết này.",
                        data = (object)null // Ép kiểu về object
                    });

                }


                return danhgia;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Danhgias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("DanhGia/{id}")]
        public async Task<IActionResult> UpdateDanhGia(int id, [FromForm] DanhgiaDTO danhgia, [FromForm] List<IFormFile> files, [FromForm] List<int> existingFileIds)
        {
            try
            {
                // 1. Cập nhật thông tin đánh giá
                await _services.Update(id, danhgia);

                // 2. Lấy danh sách hình ảnh hiện tại trong cơ sở dữ liệu
                var existingImages = await _hinhAnhService.GetByIdDanhGiaAsync(id);

                // 3. Xóa những hình ảnh không còn trong danh sách `existingFileIds`
                foreach (var image in existingImages)
                {
                    if (existingFileIds == null || !existingFileIds.Contains(image.Id))
                    {
                        // Xóa file vật lý
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture", image.Urlhinhanh);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        // Xóa bản ghi trong cơ sở dữ liệu
                        await _hinhAnhService.DeleteAsync(image.Id);
                    }
                }

                // 4. Lưu các file mới vào thư mục và cơ sở dữ liệu
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        // Tạo đường dẫn lưu file
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(folderPath, fileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Tạo đối tượng HinhanhDTO
                        var hinhanh = new HinhanhDTO
                        {
                            Idtrahang = 0,       // Tùy chỉnh theo logic của bạn
                            Iddanhgia = id,
                            Urlhinhanh = fileName
                        };

                        // Lưu thông tin hình ảnh mới vào cơ sở dữ liệu
                        await _hinhAnhService.AddAsync(hinhanh);
                    }
                }

                // 5. Trả về kết quả thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }



        // POST: api/Danhgias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DanhgiaDTO>> PostDanhgia([FromForm] DanhgiaDTO danhgia, [FromForm] List<IFormFile> files)
        {
            try
            {
                // Lưu đánh giá vào cơ sở dữ liệu
                await _services.Create(danhgia);

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        // Tạo đường dẫn lưu file
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "picture");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(folderPath, fileName);

                        // Lưu file vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Tạo đối tượng HinhanhDTO
                        var hinhanh = new HinhanhDTO
                        {
                            Idtrahang = 0,
                            Iddanhgia = danhgia.Id,
                            Urlhinhanh = fileName
                        };

                        // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                        await _hinhAnhService.AddAsync(hinhanh);
                    }
                }

                // Trả về kết quả sau khi tạo mới
                return CreatedAtAction("GetDanhgia", new { id = danhgia.Id }, danhgia);
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }





        // DELETE: api/Danhgias/5
        [HttpDelete("_KhachHang/{id}")]
        public async Task<IActionResult> DeleteDanhgia(int id)
        {
            if (await _services.GetAll() == null)
            {
                return NotFound();
            }

            try
            {
                await _services.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByIdSP/{id}")]
        public async Task<IActionResult> GetByIdSP(int id)

        {
            // Kiểm tra ID
            if (id <= 0)
            {
                return BadRequest(new { message = "ID sản phẩm chi tiết không hợp lệ." });
            }

            // Gọi dịch vụ để lấy dữ liệu
            var result = await _services.GetByidSP(id);

            // Kiểm tra kết quả trả về
            if (result == null || !result.Any())
            {
                return Ok(); // Trả về null trong response
            }

            return Ok(result); // Trả về kết quả nếu tìm thấy
        }
    }
}
