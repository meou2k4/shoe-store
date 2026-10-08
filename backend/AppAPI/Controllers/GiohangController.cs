using AppData.DTO;
using AppData.IService;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiohangController : ControllerBase
    {
        private readonly IGioHangService _Service;
        public GiohangController(IGioHangService ser)
        {
            _Service = ser;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var a = await _Service.GetAllGiohangsAsync();
            return Ok(a);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var a = await _Service.GetGiohangByIdAsync(id);
                return Ok(a);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(GiohangDTO gh)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            gh.Soluong = 0;
            await _Service.AddGiohangAsync(gh);
            return Ok("Thêm thành công!");
        }

        [HttpGet("giohangkhachhang/{id}")]
        public async Task<IActionResult> Giohangkhachhang(int id)
        {
            var hoadon = await _Service.GetByIdKHAsync(id);

            if (hoadon == null)
                return Ok(null);

            return Ok(hoadon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GiohangDTO dto)
        {
            try
            {
                await _Service.UpdateGiohangAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _Service.DeleteGiohangAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }
        }
    }
}
