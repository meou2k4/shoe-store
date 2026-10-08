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
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using AppData.IService_Admin;
using AppData.Dto_Admin;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoadonsController : ControllerBase
    {
        private readonly IHoaDonService _Service;
        private readonly IHoaDonChiTietService _HDCTservice;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ISizeRepo _Sizerepository;
        private readonly IChatLieuRepo _Chatlieurepository;
        private readonly IColorRepo _Colorrepository;
        private readonly IHoaDonChiTietService _HDCTrepository;
        private readonly ISanPhamChiTietRepo _SPCTrepository;
        private readonly ISanPhamRepo _SPrepository;
        private readonly IHoaDonRepo _repository;
        private readonly INhanVienRepo _NVrepository;
        private readonly IKhachHangRepo _KHrepository;
        private readonly IWebHostEnvironment _env;
        private readonly string _filePath;
		private readonly IHoadonService _hoadonService;

		public HoadonsController(IHoaDonService service, AppDbContext context, IHoaDonChiTietService HDCTservice, IHoaDonRepo repository, INhanVienRepo NVrepository, IKhachHangRepo KHrepository, IHoaDonChiTietService hoaDonChiTietService, IConfiguration configuration, ISanPhamChiTietRepo sPCTrepository, ISanPhamRepo sPrepository, ISizeRepo sizeRepo, IColorRepo colorRepo, IChatLieuRepo chatLieuRepo, IWebHostEnvironment env, IHoadonService hoadonService)
        {
            _Service = service;
            _context = context;
            _HDCTservice = HDCTservice;
            _configuration = configuration;
            _HDCTrepository = hoaDonChiTietService;
            _SPCTrepository = sPCTrepository;
            _SPrepository = sPrepository;
            _Sizerepository = sizeRepo;
            _Chatlieurepository = chatLieuRepo;
            _Colorrepository = colorRepo;
            _repository = repository;
            _NVrepository = NVrepository;
            _KHrepository = KHrepository;
            _env = env;
            _filePath = Path.Combine(_env.WebRootPath, "data", "read_orders.json");
            _hoadonService = hoadonService;

            // Đảm bảo thư mục tồn tại
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        [HttpPut("da-nhan-don-hang-{id}")]
        public async Task<IActionResult> Danhandonhang(int id)
        {
            try
            {
                await _Service.Danhandonhang(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // API để lấy tất cả hoá đơn
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hoadonList = await _Service.GetAllAsync();
            return Ok(hoadonList);
        }

        // API để lấy hoá đơn theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hoadon = await _Service.GetByIdAsync(id);
            if (hoadon == null) return NotFound(new { message = "Hoá đơn không tìm thấy" });
            return Ok(hoadon);
        }

        // API để thêm hoá đơn
        [HttpPost]
        public async Task<IActionResult> Add(HoadonDTO dto)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            try
            {
                if (dto.Idgg > 0)
                {
                    var giamgia = await _context.giamgias.FirstOrDefaultAsync(id => (int?)id.Id == dto.Idgg);
                    if (giamgia == null)
                    {
                        return NotFound(new { message = "Id giảm giá không tìm thấy" });
                    }
                    // Cập nhật số lượng
                    giamgia.Soluong -= 1;
                    _context.giamgias.Update(giamgia);

                    // Lưu thay đổi vào DB
                    await _context.SaveChangesAsync();
                }

                // Thêm hóa đơn
                await _Service.AddAsync(dto);

                // Trả về ID của hóa đơn mới được tạo
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi thêm hoá đơn
                return StatusCode(500, new { message = "Lỗi khi thêm hoá đơn", error = ex.Message });
            }
        }

        [HttpPost("SendOrderSuccessEmail/{id}")]
        public async Task<IActionResult> SendOrderSuccessEmail(int id)
        {
            try
            {
                // Lấy thông tin hóa đơn
                var hoadon = _context.hoadons.FirstOrDefault(a => a.Id == id);
                if (hoadon == null) return NotFound(new { message = "Hóa đơn không tồn tại" });

                // Lấy thông tin khách hàng
                var khachhang = _context.khachhangs.FirstOrDefault(a => a.Id == hoadon.Idkh);
                if (khachhang == null) return NotFound(new { message = "Khách hàng không tồn tại" });

                // Lấy danh sách hóa đơn chi tiết
                var hoadonchitiet = await _HDCTrepository.HoadonchitietTheoMaHD(hoadon.Id);
                if (!hoadonchitiet.Any()) return NotFound(new { message = "Hóa đơn không có chi tiết" });

                // Lấy danh sách sản phẩm chi tiết
                var sanphamchitietList = new List<Sanphamchitiet>();
                foreach (var ct in hoadonchitiet)
                {
                    var spct = await _SPCTrepository.GetByIdAsync(ct.Idspct);
                    if (spct != null) sanphamchitietList.Add(spct);
                }

                // Lấy danh sách sản phẩm từ SPCT
                var sanphamDict = new Dictionary<int, Sanpham>();
                foreach (var spct in sanphamchitietList)
                {
                    if (!sanphamDict.ContainsKey(spct.Idsp))
                    {
                        var sp = await _SPrepository.GetByIdAsync(spct.Idsp);
                        if (sp != null) sanphamDict[spct.Idsp] = sp;
                    }
                }

                // Kiểm tra cấu hình email
                var emailSettings = _configuration.GetSection("EmailSettings");
                var senderEmail = emailSettings["SenderEmail"] ?? throw new InvalidOperationException("Sender email not configured");
                var senderPassword = emailSettings["SenderPassword"] ?? throw new InvalidOperationException("Sender password not configured");
                var smtpServer = emailSettings["SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured");
                var smtpPort = int.Parse(emailSettings["SmtpPort"] ?? throw new InvalidOperationException("SMTP port not configured"));

                // Lấy base URL của API
                string baseUrl = $"{Request.Scheme}://{Request.Host}";

                // Tạo danh sách sản phẩm trong email
                var linkedResources = new List<LinkedResource>();
                var productListHtml = new StringBuilder();

                foreach (var ct in hoadonchitiet)
                {
                    var spct = sanphamchitietList.FirstOrDefault(x => x.Id == ct.Idspct);
                    if (spct == null || !sanphamDict.TryGetValue(spct.Idsp, out var sp)) continue;

                    var mau = await _Colorrepository.GetByIdAsync(spct.IdMau);
                    var size = await _Sizerepository.GetByIdAsync(spct.IdSize);
                    var chatlieu = await _Chatlieurepository.GetByIdAsync(spct.IdChatLieu);

                    string imageUrl = $"{baseUrl}/picture/{spct.UrlHinhanh}";
                    var imageBytes = await new HttpClient().GetByteArrayAsync(imageUrl);
                    var linkedResource = new LinkedResource(new MemoryStream(imageBytes), "image/jpeg")
                    {
                        ContentId = $"image{spct.Id}"
                    };
                    linkedResources.Add(linkedResource);

                    string priceHtml = ct.Giamgia == 0
                        ? $"<td>{ct.Giasp:#,##0 VNĐ}</td>"
                        : $"<td><del>{ct.Giasp:#,##0 VNĐ}</del> {ct.Giamgia:#,##0 VNĐ}</td>";

                    productListHtml.Append($@"
                    <tr>
                        <td><img src='cid:image{spct.Id}' width='50' height='50'></td>
                        <td>{sp.TenSanpham}</td>
                        <td>{mau?.Tenmau ?? "N/A"}</td>
                        <td>{size?.Sosize ?? 0}</td>
                        <td>{chatlieu?.Tenchatlieu ?? "N/A"}</td>
                        <td>{ct.Soluong}</td>
                        {priceHtml}
                        <td>{ct.Soluong * (ct.Giasp - (ct.Giamgia ?? 0)):#,##0 VNĐ}</td>
                    </tr>");
                }

                // Đọc template từ file HTML
                // Get the path to the template file in wwwroot/Templates
                var templatePath = Path.Combine(_env.WebRootPath, "Templates", "order_success_template.html");

                // Read the template content
                if (!System.IO.File.Exists(templatePath))
                {
                    return StatusCode(500, new { message = "Email template not found" });
                }

                var templateContent = await System.IO.File.ReadAllTextAsync(templatePath);

                // Thay thế các placeholder trong template
                var body = templateContent
                    .Replace("{khachhang.Ten}", khachhang.Ten)
                    .Replace("{hoadon.Id}", hoadon.Id.ToString())
                    .Replace("{hoadon.Thoigiandathang:dd/MM/yyyy}", hoadon.Thoigiandathang.ToString("dd/MM/yyyy"))
                    .Replace("{hoadon.Tongtiensanpham:#,##0 VNĐ}", hoadon.Tongtiensanpham.ToString("#,##0 VNĐ"))
                    .Replace("{hoadon.Tonggiamgia:#,##0 VNĐ}", (hoadon.Tonggiamgia ?? 0).ToString("#,##0") + " VNĐ")
                    .Replace("{hoadon.Phivanchuyen:#,##0 VNĐ}", hoadon.Phivanchuyen.ToString("#,##0 VNĐ"))
                    .Replace("{hoadon.Tongtiencantra:#,##0 VNĐ}", hoadon.Tongtiencantra.ToString("#,##0 VNĐ"))
                    .Replace("{productListHtml}", productListHtml.ToString())
                    .Replace("{DateTime.Now.Year}", DateTime.Now.ToString("dd/MM/yyyy"));

                // Gửi email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Xác nhận đặt hàng thành công",
                    IsBodyHtml = true
                };
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                linkedResources.ForEach(lr => htmlView.LinkedResources.Add(lr));
                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.To.Add(khachhang.Email);

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };
                await smtpClient.SendMailAsync(mailMessage);

                return Ok(new { message = "Email xác nhận đã được gửi thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi gửi email: {ex.Message}" });
            }
        }

        [HttpGet("voucher/{id}")]
        public async Task<IActionResult> Checkvoucher(int id)
        {
            // Lấy dữ liệu hóa đơn từ service
            var hoadon = await _Service.Checkvoucher(id);

            // Nếu không tìm thấy hóa đơn, trả về null
            if (hoadon == null)
                return Ok(null); // Trả về null nếu không có dữ liệu

            // Nếu có dữ liệu, trả về hóa đơn
            return Ok(hoadon);
        }


        // API để cập nhật hoá đơn
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, HoadonDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            var existingHoadon = await _Service.GetByIdAsync(id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            if (dto.Trangthaidonhang == 4)
            {
                await _HDCTservice.ReturnProductAsync(id);
            }

            try
            {
                await _Service.UpdateAsync(dto, id);
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi cập nhật hoá đơn
                return StatusCode(500, new { message = "Lỗi khi cập nhật hoá đơn", error = ex.Message });
            }
        }

        // API để cập nhật hoá đơn
        [HttpPut("trangthai/{id}")]
        public async Task<IActionResult> Updatetrangthai(int id, int trangthai)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            var existingHoadon = await _context.hoadons.FirstOrDefaultAsync(kh => kh.Id == id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            if (existingHoadon.Idgg != null && (trangthai == 4 || trangthai == 6))
            {
                var voucher = await _context.giamgias.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idgg);
                if (voucher == null)
                {
                    return NotFound(new { message = "Voucher không tìm thấy" });
                }
                // Giảm số lượng mã giảm giá
                voucher.Soluong += 1;
                _context.giamgias.Update(voucher);
                await _context.SaveChangesAsync();
            }

            if (trangthai == 4 || trangthai == 6)
            {
                await _HDCTservice.ReturnProductAsync(id);
            }

            // Cập nhật điểm khách hàng nếu trạng thái là 3 và có sử dụng điểm
            if (trangthai == 3)
            {
                var khachhang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idkh);
                if (khachhang == null)
                {
                    return NotFound(new { message = "Khách hàng không tìm thấy" });
                }

                // Tính điểm từ hoá đơn và cập nhật điểm khách hàng
                int diemhoadon = (int)existingHoadon.Tongtiencantra / 100; // Quy đổi 100 VND = 1 điểm
                khachhang.Diemsudung += diemhoadon;
                khachhang.Tichdiem += diemhoadon;

                // Kiểm tra và cập nhật rank khách hàng
                var currentRank = await _context.ranks.FirstOrDefaultAsync(r => r.Id == khachhang.Idrank);
                if (currentRank != null && khachhang.Tichdiem > currentRank.MaxMoney)
                {
                    // Tìm rank tiếp theo phù hợp với điểm hiện tại
                    var nextRank = await _context.ranks
                        .Where(r => r.MinMoney <= khachhang.Tichdiem)
                        .OrderByDescending(r => r.MinMoney)
                        .FirstOrDefaultAsync();

                    if (nextRank != null && nextRank.Id != currentRank.Id)
                    {
                        khachhang.Idrank = nextRank.Id; // Cập nhật rank mới
                    }
                }

                existingHoadon.Trangthaithanhtoan = 1;
                _context.khachhangs.Update(khachhang);
                await _context.SaveChangesAsync();
            }

            try
            {
                existingHoadon.Trangthaidonhang = trangthai;

                _context.hoadons.Update(existingHoadon);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi khi cập nhật hoá đơn",
                    error = ex.Message
                });
            }
        }

        [HttpPut("trangthaiNV1/{id}")]
        public async Task<IActionResult> UpdatetrangthaiNV1(int id, int trangthai, int idnv, DateTime? ngaygiaohang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingHoadon = await _context.hoadons.FirstOrDefaultAsync(kh => kh.Id == id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            var existingNV = await _context.nhanviens.FirstOrDefaultAsync(kh => kh.Id == idnv);
            if (existingNV == null)
            {
                return NotFound(new { message = "Nhân viên không tìm thấy" });
            }

            if (existingHoadon.Idgg != null && (trangthai == 4 || trangthai == 6))
            {
                var voucher = await _context.giamgias.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idgg);
                if (voucher == null)
                {
                    return NotFound(new { message = "Voucher không tìm thấy" });
                }
                // Giảm số lượng mã giảm giá
                voucher.Soluong += 1;
                _context.giamgias.Update(voucher);
                await _context.SaveChangesAsync();
            }

            if (trangthai == 4 || trangthai == 6)
            {
                await _HDCTservice.ReturnProductAsync(id);
            }

            // Cập nhật điểm khách hàng nếu trạng thái là 3 và có sử dụng điểm
            if (trangthai == 3)
            {
                var khachhang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idkh);
                if (khachhang == null)
                {
                    return NotFound(new { message = "Khách hàng không tìm thấy" });
                }

                // Tính điểm từ hoá đơn và cập nhật điểm khách hàng
                decimal diemGoc = (decimal)existingHoadon.Tongtiencantra / 100; // 100 VND = 1 điểm

                khachhang.Tichdiem += diemGoc;                    // Tích điểm giữ nguyên số thực
                khachhang.Diemsudung += (int)Math.Floor(diemGoc); // Điểm sử dụng chỉ lấy phần nguyên

                // Kiểm tra và cập nhật rank khách hàng
                var currentRank = await _context.ranks.FirstOrDefaultAsync(r => r.Id == khachhang.Idrank);
                if (currentRank != null && khachhang.Tichdiem > currentRank.MaxMoney)
                {
                    // Tìm rank tiếp theo phù hợp với điểm hiện tại
                    var nextRank = await _context.ranks
                        .Where(r => r.MinMoney <= khachhang.Tichdiem)
                        .OrderByDescending(r => r.MinMoney)
                        .FirstOrDefaultAsync();

                    if (nextRank != null && nextRank.Id != currentRank.Id)
                    {
                        khachhang.Idrank = nextRank.Id; // Cập nhật rank mới
                    }
                }

                existingHoadon.Trangthaithanhtoan = 1;
                _context.khachhangs.Update(khachhang);
                await _context.SaveChangesAsync();
            }

            try
            {
                existingHoadon.Trangthaidonhang = trangthai;
                existingHoadon.Idnv = idnv;

                // ➕ Gán ngày giao hàng nếu có
                if (ngaygiaohang.HasValue)
                {
                    existingHoadon.Ngaygiaothucte = ngaygiaohang.Value;
                }

                _context.hoadons.Update(existingHoadon);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi khi cập nhật hoá đơn",
                    error = ex.Message
                });
            }
        }

        [HttpPut("trangthaiNVHuy/{id}")]
        public async Task<IActionResult> UpdatetrangthaiNVHuy(int id, int trangthai, int idnv, string ghichu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingHoadon = await _context.hoadons.FirstOrDefaultAsync(kh => kh.Id == id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            var existingNV = await _context.nhanviens.FirstOrDefaultAsync(kh => kh.Id == idnv);
            if (existingNV == null)
            {
                return NotFound(new { message = "Nhân viên không tìm thấy" });
            }

            if (existingHoadon.Idgg != null && (trangthai == 4 || trangthai == 6))
            {
                var voucher = await _context.giamgias.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idgg);
                if (voucher == null)
                {
                    return NotFound(new { message = "Voucher không tìm thấy" });
                }
                // Giảm số lượng mã giảm giá
                voucher.Soluong += 1;
                _context.giamgias.Update(voucher);
                await _context.SaveChangesAsync();
            }

            if (trangthai == 4 || trangthai == 6)
            {
                await _HDCTservice.ReturnProductAsync(id);
            }

            try
            {
                existingHoadon.Trangthaidonhang = trangthai;
                existingHoadon.Idnv = idnv;
                existingHoadon.Ghichu = ghichu;

                _context.hoadons.Update(existingHoadon);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi khi cập nhật hoá đơn",
                    error = ex.Message
                });
            }
        }


        // API để cập nhật hoá đơn
        [HttpPut("trangthaiNV/{id}")]
        public async Task<IActionResult> UpdatetrangthaiNV(int id, int trangthai, int idnv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            var existingHoadon = await _context.hoadons.FirstOrDefaultAsync(kh => kh.Id == id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            var existingNV = await _context.nhanviens.FirstOrDefaultAsync(kh => kh.Id == idnv);
            if (existingNV == null)
            {
                return NotFound(new { message = "Nhân viên không tìm thấy" });
            }

            if (existingHoadon.Idgg != null && (trangthai == 4 || trangthai == 6))
            {
                var voucher = await _context.giamgias.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idgg);
                if (voucher == null)
                {
                    return NotFound(new { message = "Voucher không tìm thấy" });
                }
                // Giảm số lượng mã giảm giá
                voucher.Soluong += 1;
                _context.giamgias.Update(voucher);
                await _context.SaveChangesAsync();
            }

            if (trangthai == 4 || trangthai == 6)
            {
                await _HDCTservice.ReturnProductAsync(id);
            }

            // Cập nhật điểm khách hàng nếu trạng thái là 3 và có sử dụng điểm
            if (trangthai == 3)
            {
                var khachhang = await _context.khachhangs.FirstOrDefaultAsync(kh => kh.Id == existingHoadon.Idkh);
                if (khachhang == null)
                {
                    return NotFound(new { message = "Khách hàng không tìm thấy" });
                }

                // Tính điểm từ hoá đơn và cập nhật điểm khách hàng
                int diemhoadon = (int)existingHoadon.Tongtiencantra / 100; // Quy đổi 100 VND = 1 điểm
                khachhang.Diemsudung += diemhoadon;
                khachhang.Tichdiem += diemhoadon;

                // Kiểm tra và cập nhật rank khách hàng
                var currentRank = await _context.ranks.FirstOrDefaultAsync(r => r.Id == khachhang.Idrank);
                if (currentRank != null && khachhang.Tichdiem > currentRank.MaxMoney)
                {
                    // Tìm rank tiếp theo phù hợp với điểm hiện tại
                    var nextRank = await _context.ranks
                        .Where(r => r.MinMoney <= khachhang.Tichdiem)
                        .OrderByDescending(r => r.MinMoney)
                        .FirstOrDefaultAsync();

                    if (nextRank != null && nextRank.Id != currentRank.Id)
                    {
                        khachhang.Idrank = nextRank.Id; // Cập nhật rank mới
                    }
                }

                existingHoadon.Trangthaithanhtoan = 1;
                _context.khachhangs.Update(khachhang);
                await _context.SaveChangesAsync();
            }

            try
            {
                existingHoadon.Trangthaidonhang = trangthai;
                existingHoadon.Idnv = idnv;

                _context.hoadons.Update(existingHoadon);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi khi cập nhật hoá đơn",
                    error = ex.Message
                });
            }
        }

        // API để cập nhật hoá đơn
        [HttpPut("trangthaitrahang/{id}")]
        public async Task<IActionResult> Updatetrangthaitrahang(int id, int trangthai)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            var existingHoadon = await _context.hoadons.FirstOrDefaultAsync(kh => kh.Id == id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            try
            {
                existingHoadon.Trangthaidonhang = trangthai;

                _context.hoadons.Update(existingHoadon);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = existingHoadon.Id }, existingHoadon);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi cập nhật hoá đơn
                return StatusCode(500, new { message = "Lỗi khi cập nhật hoá đơn", error = ex.Message });
            }
        }

        // API để cập nhật hoá đơn
        [HttpPut("CheckTraHang/{id}")]
        public async Task<IActionResult> CheckTraHang(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            // B1: Lấy danh sách hóa đơn chi tiết theo `idhd`
            var hoadonchitietList = await _context.hoadonchitiets
                .Where(hdct => hdct.Idhd == id)
                .ToListAsync();

            if (hoadonchitietList == null || !hoadonchitietList.Any())
            {
                return NotFound(new { message = "Không tìm thấy danh sách hóa đơn chi tiết cho hóa đơn này" });
            }

            // B2: Kiểm tra từng hóa đơn chi tiết có tồn tại trong bảng trả hàng chi tiết
            var idsChuaTonTai = new List<int>();
            foreach (var hdct in hoadonchitietList)
            {
                var existsInTraHangChiTiet = await _context.trahangchitiets
                    .AnyAsync(thct => thct.Idhdct == hdct.Id);

                if (!existsInTraHangChiTiet)
                {
                    idsChuaTonTai.Add(hdct.Id); // Thêm idhdct chưa tồn tại vào danh sách
                }
            }

            // B3.1: Nếu tất cả hóa đơn chi tiết đều tồn tại trong bảng trả hàng chi tiết
            if (!idsChuaTonTai.Any())
            {
                return Ok(new { result = true, message = "Tất cả hóa đơn chi tiết đã tồn tại trong bảng trả hàng chi tiết" });
            }

            // B3.2: Nếu vẫn còn một số hóa đơn chi tiết chưa tồn tại
            return Ok(new
            {
                result = false,
                message = "Một số hóa đơn chi tiết chưa tồn tại trong bảng trả hàng chi tiết",
                missingIds = idsChuaTonTai
            });
        }


        // API để xóa hoá đơn
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingHoadon = await _Service.GetByIdAsync(id);
            if (existingHoadon == null)
            {
                return NotFound(new { message = "Hoá đơn không tìm thấy" });
            }

            try
            {
                await _Service.DeleteAsync(id);
                return NoContent(); // Trả về status code 204 nếu xóa thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi xóa hoá đơn
                return StatusCode(500, new { message = "Lỗi khi xóa hoá đơn", error = ex.Message });
            }
        }
        [HttpGet("hoa-don-theo-ma-kh-{id}")]
        public async Task<IActionResult> Hoadontheomakh(int id, [FromQuery] string? search)
        {
            try
            {
                return Ok(await _Service.TimhoadontheoIdKH(id, search));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi xóa hoá đơn
                return StatusCode(500, new { message = "Lỗi khi xóa hoá đơn", error = ex.Message });
            }
        }

        [HttpGet("unread-orders")]
        public IActionResult GetUnreadOrders()
        {
            var allOrders = GetBaseOrdersQuery();
            var readOrderIds = GetReadOrderIds();

            var unreadOrders = allOrders
                .Where(o => !readOrderIds.Contains(o.Id))
                .ToList();

            return Ok(unreadOrders);
        }

        [HttpGet("read-orders")]
        public IActionResult GetReadOrders()
        {
            var allOrders = GetBaseOrdersQuery();
            var readOrderIds = GetReadOrderIds();

            var readOrders = allOrders
                .Where(o => readOrderIds.Contains(o.Id))
                .ToList();

            return Ok(readOrders);
        }

        [HttpGet("old-orders")]
        public IActionResult GetOldOrders()
        {
            var cutoffDate = DateTime.Now.AddDays(-7);
            var allOrders = GetBaseOrdersQuery()
                .Where(o => o.OrderDate < cutoffDate)
                .ToList();

            return Ok(allOrders);
        }

        private IQueryable<OrderNotificationDto> GetBaseOrdersQuery()
        {
            return _context.hoadons
                .Where(x => x.Trangthaidonhang <= 4 && x.Trangthai == 0)
                .OrderByDescending(o => o.Thoigiandathang)
                .Select(o => new OrderNotificationDto
                {
                    Id = o.Id,
                    OrderCode = $"HD{o.Id.ToString().PadLeft(6, '0')}",
                    OrderDate = o.Thoigiandathang,
                    TotalAmount = o.Tongtiencantra,
                    CustomerName = o.Khachhang != null ? o.Khachhang.Ten : "Khách vãng lai",
                    Status = o.TrangthaiStr
                });
        }

        private List<int> GetReadOrderIds()
        {
            if (System.IO.File.Exists(_filePath))
            {
                var json = System.IO.File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            return new List<int>();
        }
        [HttpPost("mark-as-read")]
        public IActionResult MarkAsRead([FromBody] List<int> orderIds)
        {
            List<int> readOrders = new List<int>();

            // Đọc file nếu tồn tại
            if (System.IO.File.Exists(_filePath))
            {
                var json = System.IO.File.ReadAllText(_filePath);
                readOrders = JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }

            // Thêm ID chưa có vào
            foreach (var id in orderIds)
            {
                if (!readOrders.Contains(id))
                {
                    readOrders.Add(id);
                }
            }

            // Ghi lại vào file
            var updatedJson = JsonSerializer.Serialize(readOrders);
            System.IO.File.WriteAllText(_filePath, updatedJson);

            return Ok(new { success = true });
        }
		[HttpGet("oln/Admin")]
		public async Task<ActionResult<IEnumerable<Hoadon>>> GetAllHoadonsOln()
		{
			try
			{
				var hoadons = await _hoadonService.GetAllHoadonsOlnAsync();
				if (hoadons == null || !hoadons.Any())
				{
					return NotFound("Không tìm thấy hóa đơn nào.");
				}
				return Ok(hoadons);
			}
			catch (Exception ex)
			{
				// Ghi log lỗi hoặc trả về mã lỗi thích hợp
				return StatusCode(500, "Lỗi server: " + ex.Message);
			}
		}
        [HttpPost("create/Admin")]
        public async Task<IActionResult> CreateHoaDon([FromBody] CreateHoadonDTO dto)
        {
            try
            {
                var hoadon = await _hoadonService.AddHoaDon(dto);
                return Ok(hoadon);
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
        [HttpPost("create/khtt")]
		public async Task<IActionResult> CreateHoaDonkhtt([FromBody] HoadonoffKhachhangthanthietDto dto, [FromQuery] int diemSuDung)
		{
			try
			{
				var hoadon = await _hoadonService.AddHoaDonKhachhangthanthietoff(dto, diemSuDung);
				return Ok(hoadon);
			}
			catch (Exception ex)
			{
				// Log chi tiết inner exception
				return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
			}
		}
		[HttpGet("getall/Admin")]
		public List<Hoadon> GetAllHoadons()
		{
			var hoadons = _hoadonService.GetAllHoadons();


			return hoadons;

		}
		[HttpPut("ChuyenTrangThai/Admin")]
		public async Task<IActionResult> ChuyenTrangThai(int id, int huy)
		{
			try
			{
				var result = await _hoadonService.ChuyenTrangThaiAsync(id, huy);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpPut("RestoreState/Admin")]
		public async Task<IActionResult> RestoreState(int id, int trangthai)
		{
			try
			{
				var result = await _hoadonService.RestoreStateAsync(id, trangthai);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("Admin")]
		public async Task<IActionResult> GetByIdAdmin(int id)
		{
			try
			{
				var result = await _hoadonService.GetByIdAsync(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}
		
		[HttpGet("all/Admin")]
		public async Task<ActionResult<IEnumerable<Hoadon>>> GetAllHoadonsThongke()
		{
			try
			{
				var hoadons = await _hoadonService.GetAllHoadonsAsync();
				if (hoadons == null || !hoadons.Any())
				{
					return NotFound("Không tìm thấy hóa đơn nào.");
				}
				return Ok(hoadons);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Lỗi server: " + ex.Message);
			}
		}
		[HttpGet("Admin/Off")]
		public async Task<ActionResult<IEnumerable<Hoadon>>> GetAllOffHoadons()
		{
			try
			{
				var hoadons = await _hoadonService.GetAllOffHoadonsAsync();
				if (hoadons == null || !hoadons.Any())
				{
					return NotFound("Không tìm thấy hóa đơn nào.");
				}
				return Ok(hoadons);
			}
			catch (Exception ex)
			{
				// Ghi log lỗi hoặc trả về mã lỗi thích hợp
				return StatusCode(500, "Lỗi server: " + ex.Message);
			}
		}
		[HttpGet("daily-report/Admin")]
		public async Task<IActionResult> GetDailyReport([FromQuery] DateTime? date = null)
		{
			date ??= DateTime.Now; // Nếu không có ngày truyền vào, mặc định là ngày hôm nay
			var report = await _hoadonService.GetDailyReportAsync(date.Value);

			return Ok(new
			{
				Date = date.Value.ToString("yyyy-MM-dd"),
				TongTienThanhToan = report.TongTienThanhToan,
				TongSoLuongDonHang = report.TongSoLuongDonHang
			});
		}
		[HttpGet("order-summary/Admin")]
		public IActionResult GetOrderSummary([FromQuery] string timeUnit)
		{
			try
			{
				var summary = _hoadonService.GetOrderSummary(timeUnit);
				return Ok(summary);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}
	
		
		[HttpGet("oln-orders/Admin")]
		public async Task<IActionResult> GetOlnOrdersByWeek()
		{
			var result = await _hoadonService.GetOlnOrdersByWeekAsync();
			return Ok(result);
		}

		[HttpGet("off-orders/Admin")]
		public async Task<IActionResult> GetOffOrdersByWeek()
		{
			var result = await _hoadonService.GetOffOrdersByWeekAsync();
			return Ok(result);
		}
		[HttpGet("tong-so-don")]
		public async Task<IActionResult> TongSoDonThanhCong()
		{
			int total = await _hoadonService.GetSoDonAsync();
			return Ok(total);    
		}

		[HttpGet("tong-doanh-thu-thanh-cong")]
		public async Task<IActionResult> TongDoanhThuThanhCong()
		{
			decimal revenue = await _hoadonService.GetDoanhThuThanhCongAsync();
			return Ok(revenue);      
		}
		[HttpGet("latest-10")]
		public async Task<IActionResult> GetLatest10()
		{
			var data = await _hoadonService.Get10LatestInvoicesAsync();
			return Ok(data);     
		}
	}
}
