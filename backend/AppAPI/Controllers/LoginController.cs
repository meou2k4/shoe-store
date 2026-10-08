using AppData;
using AppData.Dto;
using AppData.IRepository;
using AppData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace AppAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly ILogger<LoginController> _logger;
        private readonly IKhachHangRepo _KhachHang_Repos;
        private readonly IGioHangRepo _KhachHang_GHrepos;
        private readonly IWebHostEnvironment _environment;

        public LoginController(AppDbContext context, ILogger<LoginController> logger, IGioHangRepo GHrepos, IKhachHangRepo repos, IWebHostEnvironment environment)
		{
			_context = context;
			_logger = logger;
			_KhachHang_GHrepos = GHrepos;
			_KhachHang_Repos = repos;
            _environment = environment;

        }

        [HttpPost("_KhachHang/register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            try
            {
                // Kiểm tra email và số điện thoại đã tồn tại chưa
                if (_context.khachhangs.Any(kh => kh.Email == dto.Email))
                {
                    return BadRequest("Email đã tồn tại");
                }
                if (_context.khachhangs.Any(kh => kh.Sdt == dto.Sdt))
                {
                    return BadRequest("Số điện thoại đã tồn tại");
                }

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
                        _logger.LogWarning(ex, "Không thể tải ảnh từ URL, sử dụng URL trực tiếp");
                        // Nếu không tải được, sử dụng URL trực tiếp
                        avatarPath = "https://i.imgur.com/3jY5r9s.png";
                    }
                }

                // Tạo tài khoản khách hàng mới
                var khachHang = new Khachhang
                {
                    Ten = dto.Ten,
                    Sdt = dto.Sdt,
                    Ngaysinh = dto.Ngaysinh,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Ngaytaotaikhoan = DateTime.UtcNow,
                    Tichdiem = 0,
                    Diemsudung = 0,
                    Gioitinh = dto.gioitinh,
                    Trangthai = 0,
                    Idrank = _context.ranks.FirstOrDefault(x => x.MinMoney == 0).Id,
                    Avatar = avatarPath // Sử dụng đường dẫn đã xử lý
                };

                await _KhachHang_Repos.AddAsync(khachHang);

                // Tạo giỏ hàng mới
                var gh = new Giohang()
                {
                    Soluong = 0,
                    Idkh = khachHang.Id
                };
                await _KhachHang_GHrepos.AddAsync(gh);

                return Ok("Đăng ký thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng ký");
            }
        }

        [HttpPost("_KhachHang/checkemail")]
        public IActionResult CheckEmail(string checkEmail)
        {
            var khachHang = _context.khachhangs.FirstOrDefault(x => x.Email == checkEmail);

            if (khachHang == null)
            {
                return Ok(new { status = "not_found", message = "Email không tồn tại." });
            }
            else if (khachHang.Password == null)
            {
                return Ok(new { status = "new_account", message = "Tài khoản chưa có mật khẩu." });
            }
            else
            {
                return Ok(new { status = "password_required", message = "Vui lòng nhập mật khẩu." });
            }
        }

        [HttpPost]
        [Route("RegisterPassword")]
        public IActionResult RegisterPassword([FromBody] LoginUserDTO request)
        {
            var user = _context.khachhangs.FirstOrDefault(x => x.Email == request.Email);
            if (user == null)
            {
                return BadRequest("Tài khoản không tồn tại.");
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Tài khoản đã có mật khẩu.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            _context.khachhangs.Update(user);
			_context.SaveChanges();

            return Ok(new { message = "Mật khẩu đã được đăng ký thành công." });
        }


        [HttpPost("_KhachHang/login")]
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
				var khachHang = await _context.khachhangs
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
					KhachHangId = khachHang.Id,
					trangthai = khachHang.Trangthai,
					Ten = khachHang.Ten,
					Email = khachHang.Email,
					Ngaytaotaikhoan = khachHang.Ngaytaotaikhoan,
                    Avatar = khachHang.Avatar
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login process");
				return StatusCode(500, "Đã xảy ra lỗi trong quá trình đăng nhập");
			}
		}

	}

}

