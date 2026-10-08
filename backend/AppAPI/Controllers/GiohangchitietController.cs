using AppData.DTO;
using AppData.IRepository;
using AppData.IService;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiohangchitietController : ControllerBase
    {
        private readonly IGioHangChiTetService _Service;
        public GiohangchitietController(IGioHangChiTetService ser)
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
                return NotFound("Giỏ hàng chi tiết không tồn tại.");
            }
        }

        [HttpGet("idghctbygiohangangspct/{idgh}/{idspct}")]
        public async Task<IActionResult> GetByIdspctToGiohangAsync(int idgh, int idspct)
        {
            try
            {
                var result = await _Service.GetByIdspctToGiohangAsync(idgh, idspct);

                if (result == null)
                {
                    return NotFound("Giỏ hàng chi tiết không tồn tại.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if needed
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(GiohangchitietDTO gh)
        {
            try
            {
                var result = await _Service.GetByIdspctToGiohangAsync(gh.Idgh, gh.Idspct);

                if (result == null)
                {
                    await _Service.AddGiohangAsync(gh);
                    return Ok("Thêm thành công!");
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpGet("giohangchitietbygiohang/{id}")]
        public async Task<IActionResult> GetDiaChiByIdKH(int id)
        {
            try
            {
                var diachiDto = await _Service.GetGHCTByIdGH(id);


                return Ok(diachiDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GiohangchitietDTO dto)
        {
            try
            {
                await _Service.UpdateGiohangAsync(id, dto);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Giỏ hàng chi tiết không tồn tại.");
            }
        }

        [HttpPut("sanpham/{id}")]
        public async Task<IActionResult> Updatesanpham(int id, [FromBody] GiohangchitietDTO dto)
        {
            try
            {
                var result = await _Service.GetByIdspctToGiohangAsync(dto.Idgh, dto.Idspct);

                if (result == null)
                {
                    await _Service.UpdateGiohangAsync(id, dto);
                    return Ok("Cập Nhật Thành Công!");
                }
                else
                {
                    await _Service.DeleteGiohangAsync(id);
                    _Service.UpdateSoLuongGiohangAsync(result.Id, dto);
                    return Ok("Sản phẩm đã tồn tại trong giỏ hàng, cập nhật thành công!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _Service.DeleteGiohangAsync(id);
                return Ok("Xoá giỏ hàng chi tiết thành công."); // Trả về thông báo khi xoá thành công
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Giỏ hàng chi tiết không tồn tại."); // Trả về lỗi nếu không tìm thấy
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}"); // Xử lý lỗi không mong muốn
            }
        }
        [HttpGet("Sodongiohangct/{id}")]
        public async Task<IActionResult> GetSodongiohangct(int id)
        {
            try
            {
                var giohangs = await _Service.GetAllGiohangsAsync();

                if (giohangs == null)
                {
                    return NotFound("Không tìm thấy giỏ hàng.");
                }

                var count = giohangs.Count(x => x.Idgh == id);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

    }
}
