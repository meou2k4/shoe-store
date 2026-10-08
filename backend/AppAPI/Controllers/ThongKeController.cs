
using AppData.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongkeService _thongkeService;
        private readonly AppData.IService_Admin.IThongkeService _thongkeService_Admin;

		public ThongKeController(IThongkeService thongkeService, AppData.IService_Admin.IThongkeService thongkeService_Admin )
        {
            _thongkeService = thongkeService;
            _thongkeService_Admin = thongkeService_Admin;
        }

        [HttpGet("top-selling-products/Admin")]
        public async Task<IActionResult> GetTopSellingProductsByTimeAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var topSellingProducts = await _thongkeService_Admin.GetTopSellingProductsByTimeAsync(startDate, endDate);
            return Ok(topSellingProducts);
        }

        [HttpGet("top5-customers/Admin")]
        public async Task<IActionResult> GetTop5Customers([FromQuery] DateTime month)
        {
            var customers = await _thongkeService.GetTop5CustomersAsync(month);
            if (customers == null || customers.Count == 0)
            {
                return NotFound("Không có dữ liệu.");
            }
            return Ok(customers);
        }
        [HttpGet("active-customers/Admin")]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var customers = await _thongkeService.GetActiveCustomersAsync();
            if (customers == null || customers.Count == 0)
            {
                return NotFound("Không có khách hàng hoạt động.");
            }
            return Ok(customers);
        }
    }
}
