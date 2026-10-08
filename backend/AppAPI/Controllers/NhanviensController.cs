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
using AppData.IRepository;
using AppData.IService;
using AppData.Dto;
using Aspose.Email.Clients.Activity;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using AppData.Dto_Admin;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanviensController : ControllerBase
    {
        private readonly INhanVienService _Service;
        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public NhanviensController(INhanVienService service, ILogger<LoginController> logger, AppDbContext context, IWebHostEnvironment environment)
        {
            _Service = service;
            _logger = logger;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _Service.GetAllNhanviensAsync();
            return Ok(result.Select(nv => new
            {
                nv.Id,
                nv.Hovaten,
                nv.Ngaysinh,
                nv.Diachi,
                nv.Email,
                nv.Gioitinh,
                nv.Sdt,
                nv.Trangthai,
                nv.Password,
                nv.Role,
                nv.Avatar,
                nv.Ngaytaotaikhoan
            }));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var nhanvien = await _Service.GetNhanvienByIdAsync(id);
                return Ok(new
                {
                    nhanvien.Id,
                    nhanvien.Hovaten,
                    nhanvien.Ngaysinh,
                    nhanvien.Diachi,
                    nhanvien.Email,
                    nhanvien.Gioitinh,
                    nhanvien.Sdt,
                    nhanvien.Trangthai,
                    nhanvien.Password,
                    nhanvien.Avatar,
                    nhanvien.Role,
                    nhanvien.Ngaytaotaikhoan
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nhân viên không tồn tại.");
            }


        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] NhanvienDTO nhanvienDto, IFormFile? avatarFile)

        {
            try
            {

                string fileName = "AnhNhanVien.png"; // Tên ảnh mặc định
                var uploadPath = Path.Combine(_environment.WebRootPath, "picture");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                if (avatarFile != null && avatarFile.Length > 0 && avatarFile.FileName != null)
                {
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                        return BadRequest("Chỉ chấp nhận ảnh định dạng .png, .jpg, .jpeg");

                    if (avatarFile.Length > 10 * 1024 * 1024)
                        return BadRequest("Kích thước ảnh tối đa là 10MB");

                    fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }
                }
                else
                {
                    // ✅ Không có ảnh -> Tải ảnh từ URL
                    string imageUrl = "https://i.pinimg.com/736x/11/5e/8a/115e8a22e7ee37d2c662d1a1714a90bf.jpg";
                    fileName = "AnhNhanVien.png";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            var imageBytes = await client.GetByteArrayAsync(imageUrl);
                            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Không thể tải ảnh mặc định: {ex.Message}");
                        }
                    }
                }

                nhanvienDto.Avatar = fileName;

                var result = await _Service.AddNhanvienAsync(nhanvienDto);

                return Ok(new
                {
                    Success = true,
                    Message = "Thêm nhân viên thành công",
                    Data = new { id = result }
                });
            }
            catch (Exception ex)
            {
                // Log the exception here (using your preferred logging method)
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi thêm nhân viên",
                    Error = ex.Message
                });
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] NhanvienUpdateDTO nhanvienDto, IFormFile? avatarFile)
        {
            try
            {
                var nhanvien = await _Service.GetNhanvienByIdAsync(id);
                if (nhanvien == null)
                    return NotFound("Nhân viên không tồn tại.");

                string fileName = nhanvien.Avatar; // giữ ảnh cũ nếu không thay đổi
                var uploadPath = Path.Combine(_environment.WebRootPath, "picture");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Xử lý khi có upload file ảnh
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Xoá ảnh cũ (nếu không phải là ảnh mặc định)
                    if (!string.IsNullOrEmpty(nhanvien.Avatar) && nhanvien.Avatar != "AnhNhanVien.png")
                    {
                        var oldPath = Path.Combine(uploadPath, nhanvien.Avatar);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                        return BadRequest("Chỉ chấp nhận ảnh định dạng .png, .jpg, .jpeg");

                    if (avatarFile.Length > 10 * 1024 * 1024)
                        return BadRequest("Kích thước ảnh tối đa là 10MB");

                    fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }
                }
                // Xử lý khi nhận URL cụ thể
                else if (nhanvienDto.Avatar == "https://i.pinimg.com/736x/11/5e/8a/115e8a22e7ee37d2c662d1a1714a90bf.jpg")
                {
                    // Xoá ảnh cũ nếu có và không phải là ảnh mặc định
                    if (!string.IsNullOrEmpty(nhanvien.Avatar) && nhanvien.Avatar != "AnhNhanVien.png")
                    {
                        var oldPath = Path.Combine(uploadPath, nhanvien.Avatar);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Set về ảnh mặc định
                    fileName = "AnhNhanVien.png";
                }

                nhanvienDto.Avatar = fileName;
                await _Service.UpdateNhanvienAsync(id, nhanvienDto);

                return Ok(new { Success = true, Message = "Cập nhật thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nhân viên không tồn tại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPut("Updatethongtin/{id}")]
        public async Task<IActionResult> Updatethongtin(int id, [FromForm] NhanvienUpdateDTO nhanvienDto, IFormFile? avatarFile)
        {
            try
            {
                var nhanvien = await _Service.GetNhanvienByIdAsync(id);
                if (nhanvien == null)
                    return NotFound("Nhân viên không tồn tại.");

                string fileName = nhanvien.Avatar; // giữ ảnh cũ nếu không thay đổi
                var uploadPath = Path.Combine(_environment.WebRootPath, "picture");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Xử lý khi có upload file ảnh
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    // Xoá ảnh cũ (nếu không phải là ảnh mặc định)
                    if (!string.IsNullOrEmpty(nhanvien.Avatar) && nhanvien.Avatar != "AnhNhanVien.png")
                    {
                        var oldPath = Path.Combine(uploadPath, nhanvien.Avatar);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                        return BadRequest("Chỉ chấp nhận ảnh định dạng .png, .jpg, .jpeg");

                    if (avatarFile.Length > 10 * 1024 * 1024)
                        return BadRequest("Kích thước ảnh tối đa là 10MB");

                    fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(stream);
                    }
                }
                // Xử lý khi nhận URL cụ thể
                else if (nhanvienDto.Avatar == "https://i.pinimg.com/736x/11/5e/8a/115e8a22e7ee37d2c662d1a1714a90bf.jpg")
                {
                    // Xoá ảnh cũ nếu có và không phải là ảnh mặc định
                    if (!string.IsNullOrEmpty(nhanvien.Avatar) && nhanvien.Avatar != "AnhNhanVien.png")
                    {
                        var oldPath = Path.Combine(uploadPath, nhanvien.Avatar);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Set về ảnh mặc định
                    fileName = "AnhNhanVien.png";
                }

                nhanvienDto.Avatar = fileName;
                await _Service.UpdateNhanvienAsync(id, nhanvienDto);

                return Ok(new { Success = true, Message = "Cập nhật thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nhân viên không tồn tại.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
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
                var khachHang = await _context.nhanviens.FirstOrDefaultAsync(kh => kh.Email == changePasswordDto.Email);
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

                _context.nhanviens.Update(khachHang);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình đổi mật khẩu", error = ex.Message });
            }
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var nhanvien = await _Service.GetNhanvienByIdAsync(id);
                if (nhanvien == null)
                    return NotFound("Nhân viên không tồn tại.");
                await _Service.DeleteNhanvienAsync(id);
                return Ok(new { Success = true, Message = "Xóa thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nhân viên không tồn tại.");
            }
        }

        [HttpPost("Nhanvien/login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {dto.Email}");
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    _logger.LogWarning("Email or password is empty");
                    return BadRequest("Email và mật khẩu không được để trống");
                }
                var khachHang = await _context.nhanviens
                    .FirstOrDefaultAsync(kh => kh.Email == dto.Email);
                if (khachHang == null)
                {
                    _logger.LogWarning($"Account not found for email: {dto.Email}");
                    return NotFound("Tài khoản không tồn tại");

                }
                bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, khachHang.Password);
                _logger.LogInformation($"Password validation result: {passwordValid}");
                if (!passwordValid)
                {
                    _logger.LogWarning($"Invalid password for email: {dto.Email}");
                    return Unauthorized("Mật khẩu không đúng");
                }
                _logger.LogInformation($"Login successful for email: {dto.Email}");
                return Ok(new
                {
                    Message = "Đăng nhập thành công",
                    id = khachHang.Id,
                    trangthai = khachHang.Trangthai,
                    Ten = khachHang.Hovaten,
                    email = khachHang.Email,
                    avatar = khachHang.Avatar,
                    role = khachHang.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng nhập");
            }
        }
		[HttpGet("tong-trangthai-0")]
		public async Task<IActionResult> TongTrangThai0()
		{
			int tong = await _Service.GetTongNhanVienTrangThai0Async();
			return Ok(tong);    
		}
		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword( [FromBody] ChangePasswordDto dto)
		{
			

			var ok = await _Service.ChangePasswordAsync(dto);
			return ok ? Ok(new { message = "Đổi mật khẩu thành công." })
					  : BadRequest(new { message = "Đổi mật khẩu thất bại. Kiểm tra mật khẩu cũ hoặc điều kiện chính sách." });
		}
		[HttpPost("send-otp")]
		public async Task<IActionResult> SendOtpAsync(ForgotPasswordRequestKHDto dto)
		{
			var (isSent, otp) = await _Service.SendOtpAsync(dto.Email); 

			if (!isSent)
				return BadRequest("Gửi OTP không thành công.");

			return Ok(new { success = true, message = "Mã OTP đã được gửi.", otp });
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });

			var result = await _Service.ResetPasswordAsync(dto);
			if (!result)
				return NotFound(new { success = false, message = "Không tìm thấy tài khoản." });

			return Ok(new { success = true, message = "Đổi mật khẩu thành công." });
		}
	}
}

