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
using AppData.IRepository;
using AppData.Dto;
using Microsoft.Extensions.Hosting;
using AppData.IService_Admin;
using AppData.Dto_Admin;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachhangsController : ControllerBase
    {
        private readonly IKhachHangService _Service;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

		private readonly IKhackhangservice _service;
		public KhachhangsController(IKhachHangService ser, AppDbContext context, IWebHostEnvironment environment, IKhackhangservice service)
        {
            _Service = ser;
            _context = context;
            _environment = environment;
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllKhachhangsAsync();
            return Ok(result
                .Where(kh => kh.Trangthai != 3)
                .Select(kh => new
            {
                kh.Id,
                kh.Ten,
                kh.Sdt,
                kh.Ngaysinh,
                kh.Tichdiem,
                kh.Email,
                kh.Password,
                kh.Diemsudung,
                Trangthai = kh.Trangthai == 0 ? "Hoạt động" : "Tài khoản bị khoá",
                kh.Idrank,
                kh.Gioitinh,
                kh.Avatar,
                kh.Ngaytaotaikhoan
            }));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var kh = await _Service.GetKhachhangByIdAsync(id);
                return Ok(new
                {
                    kh.Id,
                    kh.Ten,
                    kh.Sdt,
                    kh.Ngaysinh,
                    kh.Tichdiem,
                    kh.Email,
                    kh.Password,
                    kh.Diemsudung,
                    Trangthai = kh.Trangthai == 0 ? "Hoạt động" : "Tài khoản bị khoá",
                    kh.Idrank,
                    kh.Gioitinh,
                    kh.Avatar
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(KhachhangDTO dto)
        {
            // Xử lý avatar mặc định
            string avatarPath;
            const string defaultAvatarName = "AnhKhachHang.png";
            string physicalAvatarPath = Path.Combine(_environment.WebRootPath, "picture", defaultAvatarName);

            // Kiểm tra xem ảnh đã tồn tại trong thư mục chưa
            if (System.IO.File.Exists(physicalAvatarPath))
            {
                avatarPath = $"{defaultAvatarName}";
            }
            else
            {
                try
                {
                    // Thử tải ảnh từ URL dự phòng và lưu vào server
                    string imageUrl = "https://i.imgur.com/3jY5r9s.png"; // URL dự phòng bạn cung cấp
                    using (HttpClient client = new HttpClient())
                    {
                        // Tải ảnh từ URL
                        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);

                        // Đảm bảo thư mục tồn tại
                        Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "picture"));

                        // Lưu ảnh vào server
                        await System.IO.File.WriteAllBytesAsync(physicalAvatarPath, imageBytes);

                        avatarPath = $"{defaultAvatarName}";
                    }
                }
                catch (Exception ex)
                {
                    // Nếu không tải được, sử dụng URL trực tiếp
                    avatarPath = "https://i.imgur.com/3jY5r9s.png";
                }
            }
            dto.Avatar = avatarPath;
       
            await _Service.AddKhachhangAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = dto.Ten }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] KhachhangDTO dto)
         {
            try
            {
                await _Service.UpdateKhachhangAsync(id, dto);

                if (dto.Trangthai == 1)
                {
                    return Ok(new { message = "Tài khoản đã bị khóa." });
                }
                return Ok(new { message = "Cập nhật thông tin thành công." });
            }

            catch (KeyNotFoundException)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPut("diem/{id}")]
        public async Task<IActionResult> UpdateDiem(int id, int diemsudung)
        {
            try
            {
                var checkhachang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Id == id);
                if (checkhachang == null)
                {
                    return NotFound("Khách hàng không tồn tại.");
                }
                checkhachang.Diemsudung = diemsudung;

                _context.khachhangs.Update(checkhachang);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật thông tin thành công." });
            }

            catch (KeyNotFoundException)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPut("UpdateThongTinKhachhangAsync/{id}")]
        public async Task<IActionResult> UpdateThongTinKhachhangAsync(int id, [FromBody] KhachhangDTO dto)
        {
            try
            {
                await _Service.UpdateThongTinKhachhangAsync(id, dto);

                if (dto.Trangthai == 1)
                {
                    return Ok(new { message = "Tài khoản đã bị khóa." });
                }
                return Ok(new { message = "Cập nhật thông tin thành công." });
            }

            catch (KeyNotFoundException)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _Service.DeleteKhachhangAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Khách hàng không tồn tại.");
            }
        }
        [HttpGet("find-khachhang")]
        public async Task<IActionResult> FindByEmailAsync(string email)
        {
            var nhanVien = await _Service.FindByEmailAsync(email);
            if (nhanVien == null)
                return NotFound("Khách hàng không tồn tại.");

            return Ok(nhanVien);
        }
        // API gửi mã OTP cho quên mật khẩu
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtpAsync(ForgotPasswordRequestKHDto dto)
        {
            var (isSent, otp) = await _Service.SendOtpAsync(dto.Email); // Nhận kết quả và OTP từ service

            if (!isSent)
                return BadRequest("Gửi OTP không thành công.");

            return Ok(new { success = true, message = "Mã OTP đã được gửi.", otp }); // Trả OTP cho client (chỉ khi cần, thường dùng trong môi trường phát triển)
        }
        [HttpPost("doimatkhau")]
        public async Task<IActionResult> ChangePassword([FromBody] DoimkKhachhang changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var khachHang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Email == changePasswordDto.Email);
                if (khachHang == null)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại" });
                }

                // Kiểm tra mật khẩu cũ
                bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, khachHang.Password);
                if (!isOldPasswordValid)
                {
                    return Unauthorized(new { message = "Mật khẩu cũ không chính xác" });
                }

                // Hash mật khẩu mới
                khachHang.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                _context.khachhangs.Update(khachHang);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình đổi mật khẩu", error = ex.Message });
            }
        }
        [HttpPost("quenmatkhau")]
        public async Task<IActionResult> quenmatkhau([FromBody] DoimkKhachhang changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var khachHang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Email == changePasswordDto.Email);
                if (khachHang == null)
                {
                    return NotFound(new { message = "Tài khoản không tồn tại" });
                }

                // Kiểm tra mật khẩu cũ
                var isOldPasswordValid = khachHang.Password;

                // Hash mật khẩu mới
                khachHang.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                _context.khachhangs.Update(khachHang);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình đổi mật khẩu", error = ex.Message });
            }
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile image, [FromForm] string oldFileName = null)
        {
            try
            {
                // Kiểm tra xem có file được gửi lên không
                if (image == null || image.Length == 0)
                {
                    return BadRequest("Không có file được tải lên");
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Chỉ chấp nhận file ảnh có định dạng .png, .jpg hoặc .jpeg");
                }

                // Kiểm tra kích thước file (tối đa 10MB)
                if (image.Length > 10 * 1024 * 1024)
                {
                    return BadRequest("Kích thước file quá lớn (tối đa 10MB)");
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, "picture", oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Tạo tên file mới (sử dụng Guid để tránh trùng lặp)
                var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "picture");

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, newFileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Trả về đường dẫn tương đối của ảnh
                var imageUrl = $"/picture/{newFileName}";
                return Ok(new
                {
                    success = true,
                    imageUrl = imageUrl,
                    fileName = newFileName,
                    oldFileNameDeleted = oldFileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xử lý file: {ex.Message}");
            }
        }
		[HttpGet("{id}/Admin")]
		public async Task<ActionResult<KhachhangDto>> GetByIdAdmin(int id)
		{
			try
			{
				var khachhang = await _service.GetByIdAsyncThao(id);
				return Ok(khachhang);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		// GET: api/khachhang
		[HttpGet("Admin")]
		public async Task<ActionResult<IEnumerable<Khachhang>>> GetAll()
		{
			var khachhangs = await _service.GetAllAsyncThao();
			return Ok(khachhangs);
		}

		// POST: api/khachhang
		[HttpPost("Admin")]
		public async Task<ActionResult> Create(KhachhangDto dto)
		{
			try
			{
				await _service.CreateAsyncThao(dto);
				return CreatedAtAction(nameof(GetById), new { id = dto.Ten }, dto);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPut("Toggle/Admin")]
		public async Task<IActionResult> ToggleTrangthaiAsync(int id)  // Thêm async
		{


			try
			{
				await _service.ToggleTrangthaiAsync(id);
				return Ok(new { Message = "Cập nhật trạng thái thành công." });
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		// PUT: api/khachhang/{id}
		[HttpPut("{id}/Admin")]
		public async Task<ActionResult> Update(int id, KhachhangDto dto)
		{
			try
			{
				await _service.UpdateAsyncThao(id, dto);
				return NoContent();
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		// DELETE: api/khachhang/{id}
		[HttpDelete("{id}/Admin")]
		public async Task<ActionResult> DeleteAdmin(int id)
		{
			var result = await _service.DeleteKhachhangAsyncThao(id);
			if (result)
			{
				return Ok(new { message = "Thực hiện thành công." });
			}
			else
			{
				return NotFound(new { message = "Không tìm thấy khách hàng với Id này." });
			}
		}

		// Tìm kiếm theo tên
		[HttpGet("searchByName/Admin")]
		public async Task<ActionResult<IEnumerable<KhachhangDto>>> SearchByName(string name)
		{
			var results = await _service.SearchByNameAsyncThao(name);
			return Ok(results);
		}

		// Tìm kiếm theo số điện thoại
		[HttpGet("searchBySdt/Admin")]
		public async Task<ActionResult<IEnumerable<KhachhangDto>>> SearchBySdt(string sdt)
		{
			var results = await _service.SearchBySdtAsyncThao(sdt);
			return Ok(results);
		}

		// Tìm kiếm theo email
		[HttpGet("searchByEmail/Admin")]
		public async Task<ActionResult<IEnumerable<KhachhangDto>>> SearchByEmail(string email)
		{
			var results = await _service.SearchByEmailAsyncThao(email);
			return Ok(results);
		}
		[HttpGet("searchByEmailSdtTen/Admin")]
		public async Task<ActionResult<IEnumerable<Khachhang>>> SearchByEmailSdtTen(string keyword)
		{
			var results = await _service.SearchByEmailTenSdtAsync(keyword);
			return Ok(results);
		}
	}
}
