using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using Newtonsoft.Json;
using AppData.IService;
using AppData.Service;
using AppData.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AppData.IRepository;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHoaDonChiTietService _hoaDonChiTietService;
        private readonly IHoaDonService _hoaDonService;

        public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IHoaDonChiTietService hoaDonChiTietService, IHoaDonService hoaDonService)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _hoaDonChiTietService = hoaDonChiTietService;
            _hoaDonService = hoaDonService;
        }

        public class PaymentRequest
        {
            public int OrderCode { get; set; }
            public List<ItemRequest> Items { get; set; }
            public int TotalAmount { get; set; }
            public string Description { get; set; }
        }

        public class ItemRequest
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int Price { get; set; }
        }

        [HttpGet("/success")]
        public async Task<IActionResult> Success(int orderCode)
        {
            try
            {
                // Cập nhật trạng thái hoá đơn
                await _hoaDonService.UpdateTrangThaiChechOutAsync(orderCode, 1, 1);

                // Trả về redirect chuẩn bằng RedirectResult với status 302 (đảm bảo không dính 303)
                return Redirect("http://127.0.0.1:5501/TemplateCustomer/donhangcuaban-redirect.html");
            }
            catch (Exception ex)
            {
                // Có thể log lỗi tại đây nếu cần
                return BadRequest(new { message = "Lỗi xử lý thành công", error = ex.Message });
            }
        }


        // API xử lý thất bại
        [HttpGet("/cancel")]
        public async Task<IActionResult> Cancel(int orderCode)
        {
            try
            {
                // Cập nhật trạng thái hoá đơn
                await _hoaDonService.UpdateTrangThaiChechOutAsync(orderCode, 4, 0);
                await _hoaDonChiTietService.ReturnProductAsync(orderCode);

                return Redirect("http://127.0.0.1:5501/TemplateCustomer/donhangcuaban-redirect.html");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi xử lý thất bại", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiachi(int id)
        {
            try
            {
                var linkthanhtoan = await _payOS.getPaymentLinkInformation(id);
                if (linkthanhtoan == null) return NotFound(new { message = "Lấy thông tin link thanh toán" });
                return Ok(linkthanhtoan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> Checkout([FromBody] PaymentRequest payload)
        {
            try
            {
                // Log dữ liệu nhận từ frontend
                Console.WriteLine("Dữ liệu nhận từ FE: " + JsonConvert.SerializeObject(payload));

                // Tạo mã đơn hàng từ FE
                int orderCode = payload.OrderCode;

                // Lấy danh sách item và tổng tiền từ payload
                List<ItemData> items = payload.Items
                    .Select(i => new ItemData(i.Name, i.Quantity, i.Price))
                    .ToList();

                // Lấy URL gốc từ HTTP request
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                // Dữ liệu thanh toán
                PaymentData paymentData = new PaymentData(
                    orderCode,
                    payload.TotalAmount,
                    payload.Description,
                    items,
                    $"{baseUrl}/cancel",
                    $"{baseUrl}/success"
                );

                // Tạo link thanh toán thông qua PayOS
                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

                // Trả về link thanh toán dưới dạng JSON
                return Ok(new
                {
                    success = true,
                    checkoutUrl = createPayment.checkoutUrl
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi tạo link thanh toán.",
                    error = exception.Message
                });
            }
        }
    }
}
