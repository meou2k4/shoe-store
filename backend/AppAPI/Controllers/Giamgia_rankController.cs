using AppData.DTO;
using AppData.IService;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Giamgia_rankController : ControllerBase
    {
        private readonly IGiamGia_RankService _Service;

        public Giamgia_rankController(IGiamGia_RankService service)
        {
            _Service = service;
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
            if (hoadon == null) return NotFound(new { message = "Sale không tìm thấy" });
            return Ok(hoadon);
        }

        // API để lấy hoá đơn theo Id
        [HttpGet("rank/{id}")]
        public async Task<IActionResult> GetByIdSPCT(int id)
        {
            var hoadon = await _Service.GetByIdRankSPCTAsync(id);
            if (hoadon == null) return NotFound(new { message = "Idspct không tìm thấy" });
            return Ok(hoadon);
        }

        // API để thêm hoá đơn
        [HttpPost]
        public async Task<IActionResult> Add(giamgia_rankDTO dto)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu DTO không hợp lệ
            }

            try
            {
                // Thêm hóa đơn
                await _Service.AddAsync(dto);

                // Trả về ID của hóa đơn mới được tạo
                return CreatedAtAction(nameof(GetById), new { id = dto.Idrank }, dto);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có khi thêm hoá đơn
                return StatusCode(500, new { message = "Lỗi khi thêm hoá đơn", error = ex.Message });
            }
        }

        [HttpDelete("idgiamgia/{idgiamgia}/idrank/{idrank}")]
        public async Task<IActionResult> Delete(int idgiamgia, int idrank)
        {
            await _Service.DeleteAsync(idgiamgia, idrank);
            return NoContent();
        }

        [HttpDelete("idgg/{idgg}")]
        public async Task<IActionResult> Deletegiamgia(int idgg)
        {
            await _Service.DeletegiamgiaAsync(idgg);
            return NoContent();
        }
    }
}
