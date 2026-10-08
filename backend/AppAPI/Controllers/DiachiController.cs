using AppData;
using AppData.DTO;
using AppData.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiachiController : ControllerBase
    {
        private readonly IDiaChiService _diaChiService;
        private readonly AppDbContext _context;

        public DiachiController(IDiaChiService diaChiService, AppDbContext context)
        {
            _diaChiService = diaChiService;
            _context = context;
        }

        // GET: api/Diachis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiachiDTO>>> Getdiachis()
        {
            try
            {
                var item = await _diaChiService.GetAllDiaChi();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // GET: api/Diachis/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiachiDTO>> GetDiachi(int id)
        {
            try
            {
                var diachi = await _diaChiService.GetByIdAsync(id);
                if (diachi == null) return NotFound(new { message = "Địa chỉ không tìm thấy" });
                return Ok(diachi);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("khachhang/{id}")]
        public async Task<IActionResult> GetDiaChiByIdKH(int id)
        {
            try
            {
                var DiachiDTO = await _diaChiService.GetDiaChiByIdKH(id);

                return Ok(DiachiDTO);
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

        // PUT: api/Diachis/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiachi(int id, DiachiDTO diachi)
        {

            try
            {
                await _diaChiService.Update(id, diachi);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        // POST: api/Diachis
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DiachiDTO>> PostDiachi(DiachiDTO diachi)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _diaChiService.Create(diachi);
            return CreatedAtAction(nameof(Getdiachis), new { id = diachi.Id }, diachi);

        }

        // DELETE: api/Diachis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiachi(int id)
        {
            try
            {
                await _diaChiService.Delete(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update-default-address")]
        public IActionResult SetDefaultAddress(int idDiaChi, int idKhachHang)
        {
            try
            {
                var context = _context;
                {
                    // Tìm địa chỉ cũ đang là mặc định (trangthai = 0)
                    var oldDefaultAddress = context.diachis
                        .FirstOrDefault(a => a.Idkh == idKhachHang && a.trangthai == "0");

                    if (oldDefaultAddress != null)
                    {
                        // Chuyển trạng thái địa chỉ cũ thành 1
                        oldDefaultAddress.trangthai = "1";
                    }

                    // Tìm địa chỉ mới để đặt làm mặc định
                    var newDefaultAddress = context.diachis
                        .FirstOrDefault(a => a.Id == idDiaChi && a.Idkh == idKhachHang);

                    if (newDefaultAddress != null)
                    {
                        // Chuyển trạng thái địa chỉ mới thành 0
                        newDefaultAddress.trangthai = "0";
                    }
                    else
                    {
                        return BadRequest("Địa chỉ không tồn tại.");
                    }

                    // Lưu thay đổi vào DB
                    context.SaveChanges();
                }

                return Ok(new { message = "Cập nhật địa chỉ mặc định thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
